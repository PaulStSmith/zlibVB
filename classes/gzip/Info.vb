'**************************************************
' FILE:         Info.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Represents all the internal information 
'       of a GZIP data stream.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from gzip.vb
'
' 02    2007.OCT.06
'       Paulo Santos
'       Changed from Public to Friend
'---------------------------------------------------
' GZIP is Copyright © 1992-1993 Jean-loup Gailly
'***************************************************

Namespace GZIP

    ''' <summary>
    ''' Represents all the internal information of a GZIP data stream.
    ''' </summary>
    Friend Class Info

        Private __Method As CompressionMethod = CompressionMethod.Unknown
        Private __Flags As CompressionFlags = 0
        Private __ExtraField As Byte() = New Byte() {}
        Private __FileName As String = ""
        Private __Comments As String = ""
        Private __TimeStamp As DateTime = Nothing
        Private __PartNumber As Int16 = (-1)
        Private __HeaderBytes As Int16 = (-1)
        Private __CRC32 As Int32 = (-1)
        Private __UncompressedSize As Int32 = (-1)

        ''' <summary>
        ''' Enumerator for all the Properties of the gzipInfo
        ''' </summary>
        Friend Enum Fields As Integer
            Method = 0
            Flags
            ExtraField
            FileName
            Comments
            TimeStamp
            CRCHeader
            HeaderBytes
            CRC32
            UncompressedSize
        End Enum

        ''' <summary>
        ''' The compression method used by the GZIP compressor.
        ''' </summary>
        Public ReadOnly Property Method() As CompressionMethod
            <DebuggerHidden()> _
            Get
                Return __Method
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is an ASCII file.
        ''' </summary>
        ''' <value><c>true</c> if this instance is an ASCII file; otherwise, <c>false</c>.</value>
        Public ReadOnly Property IsAscII() As Boolean
            <DebuggerHidden()> _
            Get
                Return CBool(__Flags And CompressionFlags.AscII)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has extra field.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has extra field; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property HasExtraField() As Boolean
            <DebuggerHidden()> _
            Get
                Return CBool(__Flags And CompressionFlags.ExtraField)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has file name.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has file name; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property HasFileName() As Boolean
            <DebuggerHidden()> _
            Get
                Return CBool(__Flags And CompressionFlags.FileName)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has comment.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has comment; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property HasComment() As Boolean
            <DebuggerHidden()> _
            Get
                Return CBool(__Flags And CompressionFlags.Comment)
            End Get
        End Property

        ''' <summary>
        ''' Gets the flags.
        ''' </summary>
        ''' <value>The flags.</value>
        Public ReadOnly Property Flags() As CompressionFlags
            <DebuggerHidden()> _
            Get
                Return __Flags
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the GZIP file or stream is encrypted.
        ''' </summary>
        ''' <value>
        ''' <c>True</c> if the GZIP file or stream is encrypted; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property IsEncrypted() As Boolean
            Get
                Return CBool(__Flags And CompressionFlags.Encrypted)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the GZIP file is a continuation of a previous file.
        ''' </summary>
        ''' <value>
        ''' <c>True</c> if the GZIP file or stream is a continuation; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property IsContinuation() As Boolean
            Get
                Return CBool(__Flags And CompressionFlags.Continuation)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the GZIP file or stream has any of the reserved bit set.
        ''' </summary>
        ''' <value>
        ''' <c>True</c> if the GZIP file or stream is reserved bit set; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property IsReservedBitSet() As Boolean
            Get
                Return CBool(__Flags And CompressionFlags.Reserved)
            End Get
        End Property

        ''' <summary>
        ''' The contents of the Extra Field of the GZIP compressed data.
        ''' </summary>
        ''' <returns>A byte array with the contents of the Extra Field of the GZIP compressed data.</returns>
        Public ReadOnly Property ExtraField() As Byte()
            <DebuggerHidden()> _
            Get
                Return __ExtraField
            End Get
        End Property

        ''' <summary>
        ''' The original file name of the file compressed in the GZIP data.
        ''' </summary>
        ''' <returns>A string that contains the original file name of the file compressed in the GZIP data.</returns>
        Public ReadOnly Property FileName() As String
            <DebuggerHidden()> _
            Get
                Return __FileName
            End Get
        End Property

        ''' <summary>
        ''' The comments from the GZIP data.
        ''' </summary>
        ''' <returns>A human-readable string with the comments from the GZIP data.</returns>
        Public ReadOnly Property Comments() As String
            <DebuggerHidden()> _
            Get
                Return __Comments
            End Get
        End Property

        ''' <summary>
        ''' The date and time of the original file, or if it is missing the date and time 
        ''' when the GZIP data was generated.
        ''' </summary>
        Public ReadOnly Property TimeStamp() As DateTime
            <DebuggerHidden()> _
            Get
                Return __TimeStamp
            End Get
        End Property

        ''' <summary>
        ''' Gets the part number of the stream in a multi-part GZIP stream.
        ''' </summary>
        Public ReadOnly Property PartNumber() As Int16
            <DebuggerHidden()> _
            Get
                Return __PartNumber
            End Get
        End Property

        ''' <summary>
        ''' The number of butes of the header.
        ''' </summary>
        ''' <returns>The number of butes of the header.</returns>
        Public ReadOnly Property HeaderBytes() As Int16
            <DebuggerHidden()> _
            Get
                Return __HeaderBytes
            End Get
        End Property

        ''' <summary>
        ''' (Internal)
        ''' Updates all the read only properties of the class.
        ''' </summary>
        ''' <param name="Property">An code that will select the property to be changed.</param>
        ''' <param name="Value">The new property value.</param>
        <DebuggerStepThrough()> _
        Friend Sub [Set](ByVal [Property] As zlibVBNET.GZIP.Info.Fields, ByVal Value As Object)
            Select Case [Property]
                Case Fields.Comments : __Comments = Value
                Case Fields.CRC32 : __CRC32 = Value
                Case Fields.CRCHeader : __PartNumber = Value
                Case Fields.ExtraField : __ExtraField = Value
                Case Fields.FileName : __FileName = Value
                Case Fields.Flags : __Flags = Value
                Case Fields.HeaderBytes : __HeaderBytes = Value
                Case Fields.Method : __Method = Value
                Case Fields.TimeStamp : __TimeStamp = Value
                Case Fields.UncompressedSize : __UncompressedSize = Value
            End Select
        End Sub

    End Class

End Namespace
