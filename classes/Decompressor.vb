'**************************************************
' FILE:         Decompressor.vb
' AUTHOR:       Paulo Santos
' CREATED:      2005.JAN.10
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       zlibVBNET classes
'
' MODIFICATION HISTORY:
' 01    2005.JAN.10
'       Paulo Santos
'       Initial Version
'
' 02    2005.JAN.14
'       Paulo Santos
'       Changed GetMethod Function to comply with 
'               RFC 1952 by L. Peter Deutsch
'       Transfered the Namespace Errors to a
'               separated file.
'
' 03    2005.JAN.22
'       Paulo Santos
'       Changed the namespace from gzip to zlibNET
'       in order to accomodate further algorithms
'       implementations.
'
' 04    2007.OCT.05
'       Paulo Santos
'       Stripped and divided in multiple files.
'
' 05    2007.OCT.09
'       Paulo Santos
'       Changed the main namespace from zlibNET 
'       to zlibVBNET due the fact that there is 
'       a commercial product with the name zlib.NET.
'       Regardless my previous art, which was 
'       published at Planet Source Code in 2005.
'       As it can see in the URL below
'       http://planet-source-code.com/vb/scripts/ShowCode.asp?txtCodeId=3227&lngWId=10
'---------------------------------------------------
' This library implements the following algoritms:
'
' * GZIP (de)compression
'---------------------------------------------------
' GZIP is Copyright © 1992-1993 Jean-loup Gailly
'***************************************************

Imports zlibVBNET.GZIP
Imports zlibVBNET.Exceptions
Imports System.Collections.Generic

''' <summary>
''' Represents a zlib.NET decompressor engine.
''' </summary>
Public Class Decompressor

#Region " Private Variables "

    Private __Files As New List(Of zlibVBNET.GZIP.CompressedFile) ' <-- Files Decompressed
    Private __NumGzipParts As Integer = (-1)                    ' <-- Number of GZIP Parts in a file or stream
    Private __StreamOnly As Boolean                             ' <-- Flag indicating that the GZIP does 
    '                                                                 not contain any file, only a bunch 
    '                                                                 of compressed data.
    Private __OutputBuffer As New System.IO.MemoryStream()      ' <-- A MemoryStream to hold the output buffer

    '*
    '* These arrays are used to generate the Huffman Table by the Inflate Algorithm.
    '*
    Private __aLengthCodes() As Integer = {3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258, 0, 0}
    Private __aOffsetCodes() As Integer = {1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577}
    Private __aExBitsLengthCodes() As Integer = {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0, 99, 99}
    Private __aExBitsOffsetCodes() As Integer = {0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13}

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes an instance of the <see cref="Decompressor" /> class.
    ''' This is the default constructor for this class.
    ''' </summary>
    Public Sub New()
    End Sub

#End Region

#Region " Public Properties "

    ''' <summary>
    ''' An array of files that represents all the decompressed files from the 
    ''' compressed file or stream. It is only available if the compressed file or 
    ''' stream has file information.
    ''' </summary>
    Public ReadOnly Property Files() As zlibVBNET.GZIP.CompressedFile()
        Get
            Return __Files.ToArray()
        End Get
    End Property

    ''' <summary>
    ''' The output uncompressed stream. It is only available if the compressed 
    ''' stream has no file information.
    ''' </summary>
    Public ReadOnly Property UncompressedStream() As System.IO.Stream
        Get
            Return __OutputBuffer
        End Get
    End Property

    ''' <summary>
    ''' Indicates if the compressed file or stream has a file structure or it is 
    ''' only compressed data.
    ''' </summary>
    Public ReadOnly Property IsStreamOnly() As Boolean
        Get
            Return __StreamOnly
        End Get
    End Property

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Decompresses the specified compressed stream.
    ''' </summary>
    ''' <param name="CompressedStream">The compressed stream.</param>
    ''' <param name="force">if set to <c>true</c> attempts to force decompression even if an error occurs.</param>
    ''' <exception cref="DecompressionException">The compressed stream is invalid.
    ''' <para>- or -</para>
    ''' An unexpected error occurred while decompressing the stream occurred. Check the InnerException property of the <see cref="Exception"/> object.
    ''' </exception>
    ''' <overloads>Decompresses a stream or a byte array.</overloads>
    Public Sub Decompress(ByVal CompressedStream As System.IO.Stream, Optional ByVal force As Boolean = False)

        ' <-- Data Buffer
        Dim msData As System.IO.Stream  ' <-- Work Data Stream

        '*
        '* First we check if the stream can perform seek
        '*
        If (CompressedStream.CanSeek) Then
            '*
            '* We use the stream passed
            '*
            msData = CompressedStream
        Else
            '*
            '* Now we check if we know the stream size
            '*
            If (CompressedStream.Length = (-1)) Then
                '*
                '* Here we face a little dillema. 
                '*
                '* When we're reading a stream that cannot perform seek
                '* once we read the byte it's gone. And we need to read 
                '* the first byte in order to determine if the stream is 
                '* a valid zlibVBNET. If it's not we raise an DecompressionException 
                '* Exception and could done with it...
                '*
                '* But... (There's always a but)
                '*
                '* We also want that the caller function to be able to 
                '* threat the stream in any other way it wants once
                '* we tell him that it's an invalid zlibVBNET. 
                '*
                '* What we do?
                '*
                '* Heck... Let's just read the entire stream and, if it's
                '* not a valid GZIP, we build it back and send it on the error 
                '* information letting then caller can deal with it.
                '*
                '* Just one little problem... Reading an entire unknown length
                '* of bytes can consume the entire memory (and thus being succeptible
                '* to a buffer overrun attack)
                '*
                Dim intByte As Integer

                '*
                '* Check if we know the lenght of the stream
                '*
                Dim abytData As New System.Collections.Generic.List(Of Byte)

                '*
                '* Length unknown, so we read until the end
                '*
                intByte = CompressedStream.ReadByte()
                Do While (intByte <> (-1))
                    abytData.Add(intByte)

                    '*
                    '* Now let's be nice and tell the OS to do something else
                    '* so we do not get the "Not Responding" status
                    '*
                    Application.DoEvents()

                    '*
                    '* And to the processor
                    '*
                    If (Rnd() > 0.5) Then
                        System.Threading.Thread.Sleep(10)
                    End If

                    intByte = CompressedStream.ReadByte()
                Loop

                msData = New System.IO.MemoryStream(abytData.ToArray())
            Else
                '*
                '* Length known. Let's just read it through
                '*
                Dim abytData As Byte() = New Byte(CompressedStream.Length - 1) {}
                CompressedStream.Read(abytData, 0, CompressedStream.Length)
                msData = New System.IO.MemoryStream(abytData)
            End If
        End If

        '*
        '* Do we have something to do?
        '*
        If (msData.Length <= 2) Then
            Throw New DecompressionException(msData, My.Resources.Decompressor_InvalidCompressedStreamNoDataFound)
        End If

        '*
        '* Initialize the GZIP contents
        '*
        __StreamOnly = False

        '*
        '* Initialize the Output Buffer
        '*
        __OutputBuffer = New System.IO.MemoryStream(msData.Length * 2)

        '*
        '* Decompress the stream
        '*
        Call UnGzip(msData, force)

    End Sub

    ''' <summary>
    ''' Decompresses the specified byte array.
    ''' </summary>
    ''' <param name="ByteArray">The byte array.</param>
    ''' <param name="force">if set to <c>true</c> attempts to force decompression even if an error occurs.</param>
    Public Sub Decompress(ByVal ByteArray As Byte(), Optional ByVal force As Boolean = False)
        Me.Decompress(New System.IO.MemoryStream(ByteArray), force)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Decompress the GZIP data into files or a stream
    ''' </summary>
    ''' <param name="DataStream">The GZIP compressed data stream.</param>
    ''' <param name="force">If set to <c>true</c> attempts to force decompression even if an error occurs.</param>
    ''' <exception cref="DecompressionException">An unknown exception has occurred while decompressing the stream. See the <see cref="DecompressionException.InnerException"/>.</exception>
    ''' <exception cref="UnknownCompressionMethodException">The compression method is unknown.</exception>
    ''' <exception cref="NotImplementedCompressionMethodException">A not implemented compression method was used in the compressed stream. We're sorry for the inconvenience. Future versions of this library will address this issue.</exception>
    ''' <exception cref="InvalidStreamAndFileMixException">The compressed data stream contains an invalid mix of simple compressed bytes and compressed files.</exception>
    ''' <exception cref="ReadPastEndException">An attempt to read past the end of the stream has occurred.</exception>
    Private Sub UnGzip(ByVal DataStream As System.IO.Stream, Optional ByVal force As Boolean = False)

        Dim intMethod As CompressionMethod ' <-- Compression Method used
        Dim fileInfo As New zlibVBNET.GZIP.Info      ' <-- File information on GZIP Data

        __NumGzipParts = 0 ' <-- Initialize the parts counter
        Try
            '*
            '* Check Compression Method
            '*
            intMethod = GetMethod(DataStream, fileInfo)
            Select Case intMethod
                Case CompressionMethod.Stored, _
                     CompressionMethod.Compressed, _
                     CompressionMethod.Packed, _
                     CompressionMethod.LZH
                    Throw New NotImplementedCompressionMethodException(intMethod)
                Case CompressionMethod.Deflated
                    If (fileInfo.IsEncrypted) Then
                        '*
                        '* We don't know how to decompress encrypted streams... yet!
                        '*
                        Throw New NotImplementedCompressionMethodException(CompressionMethod.Deflated, My.Resources.Decompressor_UnGzip_EncryptedStream)
                    End If
                    If (fileInfo.IsContinuation And Not force) Then
                        '*
                        '* We don't attempt to decompress a continuation file
                        '*
                        Throw New NotImplementedCompressionMethodException(CompressionMethod.Deflated, My.Resources.Decompressor_UnGzip_InvalidContinuation)
                    End If
                    If (fileInfo.IsReservedBitSet) Then
                        '*
                        '* The reserved bits are set, so a new version of GZIP 
                        '* was used to compress it and therefore it might have 
                        '* changed how the bytes are interpreted.
                        '*
                        Throw New NotImplementedCompressionMethodException(CompressionMethod.Deflated, My.Resources.Decompressor_UnGzip_ReservedBitSet)
                    End If

                Case CompressionMethod.Unknown
                    Throw New UnknownCompressionMethodException
            End Select

            '*
            '* If the GetMethod filled the FileInfo structure, 
            '* then we have at least one file inside the GZIP 
            '* stream... If not the GZIP is only an compressed
            '* data stream.
            '*
            '* The two types cannot be mixed.
            '*
            __StreamOnly = (fileInfo.FileName = "")

            Try
                Dim decompressedBytes() As Byte ' <-- Decompressed Byte Buffer

                '*
                '* Now we process the stream accordingly
                '*
                Do While (DataStream.Position < DataStream.Length)
                    decompressedBytes = New Byte() {} ' <-- Initialize the Buffer

                    Select Case intMethod
                        Case CompressionMethod.Deflated
                            '*
                            '* Default GZIP compression
                            '*
                            decompressedBytes = Inflate(DataStream)

                            '*
                            '* Transfers the decompressed bytes to the file structure
                            '*
                            If (Not __StreamOnly) Then
                                Dim f As New zlibVBNET.GZIP.CompressedFile
                                With f
                                    .SetFileName(fileInfo.FileName)
                                    .SetContent(decompressedBytes)
                                    .SetTimeStamp(fileInfo.TimeStamp)
                                End With
                                __Files.Add(f)
                                __OutputBuffer = New System.IO.MemoryStream ' <-- Clear the output buffer for the next file
                            End If
                    End Select

                    '*
                    '* Do we have something else to process?
                    '*
                    If (DataStream.Position >= DataStream.Length) Then
                        Exit Do
                    End If

                    '*
                    '* If is there anything more to decompress
                    '*
                    Try
                        intMethod = GetMethod(DataStream, fileInfo)
                        If (intMethod = CompressionMethod.Unknown) Then
                            '*
                            '* Some system generates garbage after the valid data... 
                            '* so we just ignore them
                            '*
                            Exit Do
                        End If

                        '*
                        '* Here we check for Invalid Mix of File and Stream Only
                        '*
                        If ((fileInfo.FileName = "") AndAlso (Not __StreamOnly)) OrElse _
                           ((fileInfo.FileName <> "") AndAlso (__StreamOnly)) Then
                            '*
                            '* Check if something was decompressed before
                            '*
                            If (decompressedBytes.Length <> 0) Then
                                Throw New InvalidStreamAndFileMixException(My.Resources.Decompressor_UnGzip_InvalidStreamAndFileMix_SomeDataWasDecompressed)
                            Else
                                Throw New InvalidStreamAndFileMixException
                            End If
                        End If
                    Catch ex As ReadPastEndException
                        '*
                        '* Once we already have something decompressed we don't give any warning
                        '*
                        Exit Do
                    Catch
                        Throw
                    End Try
                Loop
            Catch
                Throw
            End Try
        Catch ex As System.Exception
            Throw New DecompressionException(DataStream, My.Resources.Decompressor_UnGzip_InvalidGzipFileStream, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Try to discover what is the compression method used by the GZIP compressor.
    ''' </summary>
    ''' <param name="DataStream">A <see cref="System.IO.Stream" /> object with the compressed data.</param>
    ''' <param name="ExtraInfo">(Optional) An <see cref="zlibVBNET.GZIP.Info" /> object to collect the information read from the GZIP data.</param>
    ''' <returns>The compression method used by the GZIP compressor.</returns>
    ''' <exception cref="DecompressionException">The compressed stream is invalid.</exception>
    ''' <exception cref="ReadPastEndException">An attempt to read past the end of the stream occurred.</exception>
    Private Function GetMethod(ByVal DataStream As System.IO.Stream, Optional ByRef ExtraInfo As zlibVBNET.GZIP.Info = Nothing) As CompressionMethod

        Dim intMethod As CompressionMethod = CompressionMethod.Unknown
        Dim objExtraInfo As New zlibVBNET.GZIP.Info
        __NumGzipParts += 1 ' <-- Add the Part Counter

        '*
        '* Now we check the first byte of the stream
        '*
        Try
            If (GetByte(DataStream) = 31) Then ' <-- 31 = 0x1f = \037
                '*
                '* So far so good... Now we check the second byte
                '*
                Select Case (GetByte(DataStream))
                    Case 30                         ' <-- 30 = 0x1e = \036
                        '*
                        '* It's a Packed Stream
                        '*
                        intMethod = CompressionMethod.Packed
                    Case 139, 158                   ' <-- 139 = 0x8b = \213
                        '                                 158 = 0x9e = \236  
                        '*
                        '* It's a GZIP Stream
                        '*

                        Dim intFlags As Integer
                        Dim intTimeStamp As Integer
                        Dim intExtraFieldLength As Integer
                        Dim strFileName As String
                        Dim strComments As String
                        Dim intStartStream As Integer

                        intStartStream = DataStream.Position - 2
                        intMethod = GetByte(DataStream)

                        intFlags = GetByte(DataStream)
                        objExtraInfo.Set(GZIP.Info.Fields.Flags, intFlags)

                        '*
                        '* Read the TimeStamp
                        '*
                        intTimeStamp = GetByte(DataStream)
                        intTimeStamp += GetByte(DataStream) << 8
                        intTimeStamp += GetByte(DataStream) << 16
                        intTimeStamp += GetByte(DataStream) << 24
                        objExtraInfo.Set(GZIP.Info.Fields.TimeStamp, New Date(1970, 1, 1).AddSeconds(intTimeStamp))

                        GetByte(DataStream) ' <-- Ignore Extra Flag
                        GetByte(DataStream) ' <-- Ignore OS Type

                        If ((intFlags And CompressionFlags.Continuation) <> 0) Then
                            objExtraInfo.Set(GZIP.Info.Fields.CRCHeader, GetByte(DataStream))
                            objExtraInfo.Set(GZIP.Info.Fields.CRCHeader, objExtraInfo.PartNumber + GetByte(DataStream) << 8)
                        End If

                        If ((intFlags And CompressionFlags.ExtraField) <> 0) Then
                            intExtraFieldLength = GetByte(DataStream)
                            intExtraFieldLength += GetByte(DataStream) << 8

                            '*
                            '* Skip the two bytes that are the one's complement of the Field Length (RFC1952)
                            '*
                            Call GetByte(DataStream)
                            Call GetByte(DataStream)

                            '*
                            '* Ignoring Extra Field
                            '*
                            Dim abytExtraField(intExtraFieldLength - 1) As Byte
                            Do While (intExtraFieldLength)
                                abytExtraField(abytExtraField.Length - intExtraFieldLength - 1) = GetByte(DataStream)
                                intExtraFieldLength -= 1
                            Loop
                            objExtraInfo.Set(GZIP.Info.Fields.ExtraField, abytExtraField)
                        End If

                        If ((intFlags And CompressionFlags.FileName) <> 0) Then
                            '*
                            '* Read the File Name
                            '*
                            Dim intByte As Byte
                            strFileName = ""
                            Do
                                intByte = GetByte(DataStream)
                                If (intByte <> 0) Then strFileName &= Chr(intByte)
                            Loop While (intByte <> 0)
                            objExtraInfo.Set(GZIP.Info.Fields.FileName, strFileName)
                        End If

                        If ((intFlags And CompressionFlags.Comment) <> 0) Then
                            '*
                            '* Read the Comment
                            '*
                            Dim intByte As Byte
                            strComments = ""
                            Do
                                intByte = GetByte(DataStream)
                                If (intByte <> 0) Then strComments &= Chr(intByte)
                            Loop While (intByte <> 0)
                            objExtraInfo.Set(GZIP.Info.Fields.Comments, strComments)
                        End If

                        objExtraInfo.Set(GZIP.Info.Fields.HeaderBytes, (DataStream.Position - intStartStream))
                    Case 157 ' <-- 157 = 0x9d = \235
                        '*
                        '* It's a LZH Stream
                        '*
                        intMethod = CompressionMethod.Compressed
                    Case 160 ' <-- 160 = 0xad = \240
                        '*
                        '* It's a LZH Stream
                        '*
                        intMethod = CompressionMethod.LZH
                    Case Else
                        '*
                        '* Sorry... Don't know it
                        '*
                        Return CompressionMethod.Unknown
                End Select

                '*
                '* If we found a method we return it
                '*
                If (intMethod >= (0)) Then
                    ExtraInfo = objExtraInfo
                    Return (intMethod)
                End If

                '*
                '* If it's the first member then it's an invalid stream
                '*
                If (__NumGzipParts = 1) Then
                    Throw New DecompressionException(DataStream, My.Resources.Decompressor_UnGzip_InvalidGzipFileStream)
                End If
            Else
                '*
                '* Invalid GZIP
                '*
                Return CompressionMethod.Unknown
            End If
        Catch ex As ReadPastEndException
            Throw New DecompressionException(DataStream, My.Resources.ReadPastEnd_Message, ex)
        Catch
            Throw
        End Try

    End Function

    ''' <summary>
    ''' Decompress a data stream compressed by the Deflate algorithm.
    ''' <p />
    ''' This implementation follow the guidelines stated by RFC1952.
    ''' </summary>
    ''' <param name="DataStream">The compressed stream.</param>
    ''' <returns>An <see cref="Array"/> of <see cref="Byte"/> that is the representation of the decompressed DataStream.</returns>
    Private Function Inflate(ByVal DataStream As System.IO.Stream) As Byte()

        Dim BitStream As zlibVBNET.BitStream          ' <-- A stream of bits from the DataStream
        Dim flgLastBlock As Boolean                 ' <-- Flag indicating the last block
        Dim bytBlockType As Byte                    ' <-- Block Type

        BitStream = New zlibVBNET.BitStream(DataStream)
        BitStream.BitOrder = BitStream.ReadOrder.LSB

        Do
            Try
                flgLastBlock = CBool(BitStream.ReadBits(1))
                bytBlockType = BitStream.ReadBits(2)
            Catch ex As ReadPastEndException
                Exit Do
            Catch
                Throw
            End Try

            Select Case bytBlockType
                Case 0
                    '*
                    '* No Compression
                    '*
                    Call InflateNoCompression(BitStream)
                Case 1
                    '*
                    '* Fixed Huffman Codes
                    '*
                    Call InflateStatic(BitStream)
                Case 2
                    '*
                    '* Dynamic Huffman Codes
                    '*
                    Call InflateDynamic(BitStream)
            End Select
        Loop Until (flgLastBlock OrElse ((BitStream.InnerStream.Position = BitStream.InnerStream.Length)))

        Return __OutputBuffer.ToArray()

    End Function

#Region " Inflate Helper Functions "

    ''' <summary>
    ''' Inflates a block with no compression.
    ''' </summary>
    ''' <param name="BitStream">A <see cref="zlibVBNET.BitStream" />  object that contains the data to be inflated.</param>
    ''' <exception cref="DecompressionException">The length of the block did not pass the verification.</exception>
    Private Sub InflateNoCompression(ByRef BitStream As zlibVBNET.BitStream)

        Dim intLenBlock As Long                             ' <-- Length of the block
        Dim intLenBlock1Complement As Long                  ' <-- One Complement of the Length

        BitStream.DumpBits(BitStream.BitsInBuffer And 7)    ' <-- Ignore the entry blocks
        intLenBlock = BitStream.ReadBits(16)                ' <-- Read the Length of the Block
        intLenBlock1Complement = BitStream.ReadBits(16)     ' <-- Read the One Complement

        If (intLenBlock <> ((Not intLenBlock1Complement) And &HFFFF)) Then
            Throw New DecompressionException(My.Resources.Decompressor_InflateNoCompression_DataBlockLengthDidNotPassedVerification)
        End If

        Do While (intLenBlock)
            __OutputBuffer.WriteByte(BitStream.ReadBits(8))
            intLenBlock -= 1
        Loop

    End Sub

    ''' <summary>
    ''' Inflates a block compressed with a Static Huffman Table
    ''' </summary>
    ''' <param name="BitStream">A <see cref="zlibVBNET.BitStream" /> object that contains the data to be inflated.</param>
    Private Sub InflateStatic(ByRef BitStream As zlibVBNET.BitStream)

        Dim aLiteralCodes(288) As Integer
        Dim aOffsetCodes(32) As Integer

        Dim oHuffOffsetCodes As zlibVBNET.GZIP.HuffmanTable
        Dim oHuffLiteralCodes As zlibVBNET.GZIP.HuffmanTable

        '*
        '* Fills the Length for the Literal/Length Codes
        '*
        For i As Integer = 0 To 144
            aLiteralCodes(i) = 8
        Next
        For i As Integer = 144 To 256
            aLiteralCodes(i) = 9
        Next
        For i As Integer = 256 To 280
            aLiteralCodes(i) = 7
        Next
        For i As Integer = 280 To 288
            aLiteralCodes(i) = 8
        Next

        '*
        '* Fills the Length for the Offset Codes
        '*
        For i As Integer = 0 To 30
            aOffsetCodes(i) = 5
        Next

        '*
        '* Builds the Huffman Table for Literal Codes
        '*
        oHuffLiteralCodes = New GZIP.HuffmanTable()
        With oHuffLiteralCodes
            .NumberSimpleCodes = 257                        ' <-- Number of simple codes
            .CodeCount = 288                                ' <-- Number of valid codes
            .LengthCodeArray = aLiteralCodes                ' <-- Array with all length codes for the distance table
            .BaseValuesArray = Me.__aOffsetCodes            ' <-- The meaning of the value for each code
            .ExtraBitsArray = Me.__aExBitsLengthCodes       ' <-- The Extra Bits needed for each code    
            Call .GenerateTable(GZIP.HuffmanTable.BitOrder.Reverse)
        End With

        '*
        '* Builds the Huffman Table for Offset/Length Codes
        '*
        oHuffOffsetCodes = New GZIP.HuffmanTable()
        With oHuffOffsetCodes
            .NumberSimpleCodes = 0                          ' <-- Number of simple codes
            .CodeCount = 30                                 ' <-- Number of valid codes
            .LengthCodeArray = aOffsetCodes                 ' <-- Array with all length codes for the Offset/Length table
            .BaseValuesArray = __aOffsetCodes               ' <-- The meaning of the value for each code
            .ExtraBitsArray = __aExBitsOffsetCodes          ' <-- The Extra Bits needed for each code    
            Call .GenerateTable(GZIP.HuffmanTable.BitOrder.Reverse)
        End With

        '*
        '* Ok... Now we have enough to actually do something.
        '*
        Call InflateCodes(BitStream, oHuffLiteralCodes, oHuffOffsetCodes)

    End Sub

    ''' <summary>
    ''' Inflates a block compressed with a Dynamic Huffman Table
    ''' </summary>
    ''' <param name="BitStream">A <see cref="zlibVBNET.BitStream" />  object that contains the data to be inflated.</param>
    ''' <exception cref="DecompressionException">The compressed stream has an invalid number of literal length codes.
    ''' <para>- or -</para>The compressed stream has an invalid number of distance codes.</exception>
    Private Sub InflateDynamic(ByRef BitStream As zlibVBNET.BitStream)

        Dim aBitLength(19) As Integer
        Dim aBitLengthOrder() As Byte = {16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15}

        Dim intNumLiteralLengthCodes As Integer
        Dim intNumDistanceCodes As Integer
        Dim intNumBitLengthCodes As Integer

        Dim oHuff As zlibVBNET.GZIP.HuffmanTable
        Dim oHuffOffsetCodes As zlibVBNET.GZIP.HuffmanTable
        Dim oHuffLiteralCodes As zlibVBNET.GZIP.HuffmanTable

        Dim intLookupBits As Integer
        Dim aLiteralLengthValues(288 + 32) As Integer
        Dim aLiteralCodes() As Integer = {}
        Dim aOffsetCodes() As Integer = {}

        Dim intDistanceBits As Integer = 9
        Dim intLengthBits As Integer = 6

        '*
        '* Read the values from the stream
        '*
        BitStream.BitOrder = BitStream.ReadOrder.LSB
        intNumLiteralLengthCodes = 257 + BitStream.ReadBits(5)
        intNumDistanceCodes = 1 + BitStream.ReadBits(5)
        intNumBitLengthCodes = 4 + BitStream.ReadBits(4)

        '*
        '* Check the Values read
        '*
        If (intNumLiteralLengthCodes > 286) Then
            Throw New DecompressionException(My.Resources.Decompressor_InflateDynamic_InvalidNumberOfLiteralLengthCodes)
        End If

        If (intNumDistanceCodes > 30) Then
            Throw New DecompressionException(My.Resources.Decompressor_InflateDynamic_InvalidNumberOfDistanceCodes)
        End If

        '*
        '* Read the Literal/Length Values
        '*
        BitStream.BitOrder = BitStream.ReadOrder.LSB
        For A As Integer = 0 To (intNumBitLengthCodes - 1)
            aBitLength(aBitLengthOrder(A)) = BitStream.ReadBits(3)
        Next

        '*
        '* Builds the Literal/Length Table Lookup
        '*
        intLookupBits = 7
        oHuff = New zlibVBNET.GZIP.HuffmanTable
        With oHuff
            .LengthCodeArray = aBitLength
            .MaxLookupBits = intLookupBits
            .CodeCount = 19
            .NumberSimpleCodes = 19
        End With
        Call oHuff.GenerateTable(GZIP.HuffmanTable.BitOrder.Reverse)

        '*
        '* Reads the literal and distance code lengths
        '*
        BitStream.BitOrder = BitStream.ReadOrder.LSB
        Dim oValue As zlibVBNET.GZIP.HuffmanTable.DecodedValue
        For A As Integer = 0 To ((intNumLiteralLengthCodes + intNumDistanceCodes) - 1)
            Dim intAux As Integer
            Dim intLastLength As Integer

            '*
            '* Read a Literal or Distance Code Lenght
            '*
            oValue = GetValueFromBitStream(BitStream, oHuff)
            Select Case oValue.Value
                Case Is < 16
                    '*
                    '* Length of the code in bits
                    '*
                    intLastLength = oValue.Value
                    aLiteralLengthValues(A) = oValue.Value
                Case 16
                    '*
                    '* Repeat last length 3 to 6 times
                    '*
                    intAux = 3 + BitStream.ReadBits(2)
                    Do While (intAux)
                        aLiteralLengthValues(A) = intLastLength
                        A += 1
                        intAux -= 1
                    Loop
                    A -= 1 ' <-- To compensate For increment
                Case 17
                    '*
                    '* 3 to 10 zero length codes
                    '*
                    intAux = 3 + BitStream.ReadBits(3)
                    Do While (intAux)
                        aLiteralLengthValues(A) = 0
                        A += 1
                        intAux -= 1
                    Loop
                    A -= 1 ' <-- To compensate For increment
                Case 18
                    '*
                    '* 11 to 138 zero length codes
                    '*
                    intAux = 11 + BitStream.ReadBits(7)
                    Do While (intAux)
                        aLiteralLengthValues(A) = 0
                        A += 1
                        intAux -= 1
                    Loop
                    A -= 1 ' <-- To compensate For increment
            End Select
        Next

        '*
        '* Builds the Huffman Table for Literal Codes
        '*
        ReDim aLiteralCodes(intNumLiteralLengthCodes)
        Array.Copy(aLiteralLengthValues, aLiteralCodes, intNumLiteralLengthCodes)
        oHuffLiteralCodes = New GZIP.HuffmanTable
        With oHuffLiteralCodes
            .NumberSimpleCodes = 257                    ' <-- Number of simple codes
            .CodeCount = intNumLiteralLengthCodes       ' <-- Number of valid codes
            .LengthCodeArray = aLiteralCodes            ' <-- Array with all length codes for the distance table
            .BaseValuesArray = __aLengthCodes           ' <-- The meaning of the value for each code
            .ExtraBitsArray = __aExBitsLengthCodes      ' <-- The Extra Bits needed for each code    
            Call .GenerateTable(GZIP.HuffmanTable.BitOrder.Reverse)
        End With

        '*
        '* Builds the Huffman Table for Offset/Length Codes
        '*
        ReDim aOffsetCodes(intNumDistanceCodes)
        Array.Copy(aLiteralLengthValues, intNumLiteralLengthCodes, aOffsetCodes, 0, intNumDistanceCodes)
        oHuffOffsetCodes = New GZIP.HuffmanTable
        With oHuffOffsetCodes
            .NumberSimpleCodes = 0                      ' <-- Number of simple codes
            .CodeCount = intNumDistanceCodes            ' <-- Number of valid codes
            .LengthCodeArray = aOffsetCodes             ' <-- Array with all length codes for the Offset/Length table
            .BaseValuesArray = __aOffsetCodes           ' <-- The meaning of the value for each code
            .ExtraBitsArray = __aExBitsOffsetCodes      ' <-- The Extra Bits needed for each code    
            Call .GenerateTable(GZIP.HuffmanTable.BitOrder.Reverse)
        End With

        '*
        '* Ok... Now we have enough to actually do something.
        '*
        Call InflateCodes(BitStream, oHuffLiteralCodes, oHuffOffsetCodes)

    End Sub

    ''' <summary>
    ''' Reads the bits from the stream decoding them and writes the proper values to the output buffer.
    ''' </summary>
    ''' <param name="BitStream">The <see cref="zlibVBNET.BitStream"/> from where the bits will be read.</param>
    ''' <param name="LiteralHuffTable">A <see cref="zlibVBNET.GZIP.HuffmanTable"/> that decodes the bits to literal values.</param>
    ''' <param name="OffsetHuffTable">A <see cref="zlibVBNET.GZIP.HuffmanTable"/> that decodes the bits to offset values.</param>
    Private Sub InflateCodes(ByRef BitStream As zlibVBNET.BitStream, ByVal LiteralHuffTable As zlibVBNET.GZIP.HuffmanTable, ByVal OffsetHuffTable As zlibVBNET.GZIP.HuffmanTable)

        Dim oValue As zlibVBNET.GZIP.HuffmanTable.DecodedValue

        Do
            oValue = GetValueFromBitStream(BitStream, LiteralHuffTable)

            If (oValue.ExtraBits = 16) Then
                '*
                '* It's a literal, so just put the bits on the output
                '*
                __OutputBuffer.WriteByte(oValue.Value)
            ElseIf (oValue.ExtraBits = 15) Then
                '*
                '* End of Block, so we get out
                '*
                Exit Do
            Else
                '*
                '* Get how many bytes we will copy
                '*
                Dim intLength As Integer
                intLength = oValue.Value + (BitStream.ReadBits(oValue.ExtraBits))

                '*
                '* Decode distance of block to copy
                '*
                Dim oDistance As zlibVBNET.GZIP.HuffmanTable.DecodedValue
                oDistance = GetValueFromBitStream(BitStream, OffsetHuffTable)

                Dim intDistance As Integer
                intDistance = oDistance.Value + BitStream.ReadBits(oDistance.ExtraBits)

                '*
                '* Copy the bytes from the output buffer
                '*
                Do While (intLength)
                    Dim aBytesToCopy() As Byte
                    Dim intBytesRead As Integer

                    ReDim aBytesToCopy(intLength)
                    __OutputBuffer.Seek(-intDistance, IO.SeekOrigin.Current)
                    intBytesRead = __OutputBuffer.Read(aBytesToCopy, 0, intLength)
                    __OutputBuffer.Seek(intDistance - intBytesRead, IO.SeekOrigin.Current)
                    __OutputBuffer.Write(aBytesToCopy, 0, intBytesRead)
                    intLength -= intBytesRead
                Loop
            End If
            System.Threading.Thread.Sleep(New TimeSpan(100))
        Loop Until (BitStream.InnerStream.Position = BitStream.InnerStream.Length)

    End Sub

#End Region

#Region " Debug "
#If DEBUG Then
    Private Sub DumpText(ByVal text As String)
        Dim aBytes() As Byte
        Dim fileLog As New System.IO.FileStream("DUMP", IO.FileMode.Append, IO.FileAccess.Write)
        aBytes = System.Text.Encoding.ASCII.GetBytes(text & vbCrLf)
        fileLog.Write(aBytes, 0, aBytes.Length)
        fileLog.Close()
    End Sub
#End If
#End Region

    ''' <summary>
    ''' Retrieves a meaningful value from the specified bit stream, based on the specified Huffman Table.
    ''' </summary>
    ''' <param name="BitStream">A <see cref="zlibVBNET.BitStream"/> object from where retrieve the bits in order to find a match on the specified Huffman Table.</param>
    ''' <param name="HuffmanTable">A <see cref="zlibVBNET.GZIP.HuffmanTable"/> object that contains the information about how to decode the bits from the stream.</param>
    ''' <returns>A <see cref="zlibVBNET.GZIP.HuffmanTable.DecodedValue"/> object that is the representation of the meaning of the bits from the compressed bit stream.</returns>
    Private Function GetValueFromBitStream(ByRef BitStream As zlibVBNET.BitStream, ByVal HuffmanTable As GZIP.HuffmanTable) As GZIP.HuffmanTable.DecodedValue

        Dim intCode As Integer
        Dim oValue As GZIP.HuffmanTable.DecodedValue

        '*
        '* Attempts to retrieve a meaningful code from the BitStream
        '*
        For intBits As Integer = HuffmanTable.MinLookupBits To HuffmanTable.MaxLookupBits
            '*
            '* Gets a few number of bits from the stream
            '* Remember that GetBits just put the bits into the bit buffer.
            '*
            intCode = BitStream.GetBits(intBits)
            oValue = HuffmanTable.DecoderTable.Item(intCode)

            '*
            '* We also double check the number of bits to 
            '* avoid ambiguity.
            '*
            '* For instance if we have a MinLookupBits = 2
            '* it could misinterprete the code 0011 as being 
            '* the code 11 that in such case could not occur.
            '*
            If (Not oValue Is Nothing) AndAlso (oValue.NumBits = intBits) Then
                '*
                '* Ok. We've got a match
                '*
                BitStream.DumpBits(intBits) ' <-- Consume the bits
                Return oValue               ' <-- Return the value found
            End If
        Next

        '*
        '* Keeps the compiler happy
        '*
        Return Nothing

    End Function

#End Region

End Class
