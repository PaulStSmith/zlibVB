'**************************************************
' FILE:         BitStream.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Provides a way of streaming bits, 
'       instead of bytes, from a System.IO.Stream object.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from gzip.vb
'***************************************************

''' <summary>
''' Provides a way of streaming bits, instead of bytes, from a <see cref="System.IO.Stream"/> object.
''' </summary>
Public Class BitStream

#Region " Enumerators "

    ''' <summary>
    ''' Enumerates the possible order of how the bits and bytes are stored in the underlieing stream of a BitStream object.
    ''' </summary>
    Public Enum ReadOrder As Byte
        ''' <summary>
        ''' Most Significant Bit or Byte.
        ''' </summary>
        MSB = 0

        ''' <summary>
        ''' Least Significant Bit or Byte.
        ''' </summary>
        ''' <remarks></remarks>
        LSB
    End Enum

#End Region

#Region " Private Variables "

    Private __Stream As System.IO.Stream ' <-- The stream from where the bits are read
    Private __BitBuffer As Long          ' <-- A Buffer for the bits read
    Private __NumBits As Byte            ' <-- Number of bits in the buffer
    Private __BitOrder As ReadOrder      ' <-- The order that the bits will be interpreted.
    Private __ByteOrder As ReadOrder     ' <-- The order how the bytes are stored in the original stream.

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the BitStream Class.
    ''' </summary>
    ''' <param name="BaseStream">A <see cref="System.IO.Stream" /> object that will provide the underlieing stream where to read the bits from.</param>
    ''' <exception cref="ArgumentNullException">The argument <paramref name="BaseStream" /> is <langword name="null" />.</exception>
    Public Sub New(ByVal BaseStream As System.IO.Stream)
        If (BaseStream Is Nothing) Then
            Throw New ArgumentNullException("BaseStream")
        End If

        __BitOrder = ReadOrder.MSB
        __ByteOrder = ReadOrder.LSB
        __Stream = BaseStream
    End Sub

#End Region

#Region " Public Properties "

    ''' <summary>
    ''' Gets or sets the how the bits on the bytes read are interpreted.
    ''' </summary>
    ''' <remarks>
    ''' When setted to MSB all calls to functions that retrieve bits from the 
    ''' stream returns first the Most Significant Bits.
    ''' <p />
    ''' Likewise, when setted to LSB all calls returns first the Least Significant 
    ''' Bits.
    ''' <p />
    ''' Because the BitStream object cannot read less than a byte from the original 
    ''' stream be aware that a reading bits from the stream might cross the byte 
    ''' boundary. To ensure the most correct answer possible make sure to set 
    ''' both BitOrder and ByteOrder properties to avoid unexpected behaviors.
    ''' <p />
    ''' By default the BitOrder is setted to MSB.
    ''' </remarks>
    ''' <example>
    ''' <code title="The example below considers this stream as its source">
    ''' 01101010 01010101 11110000 10101010
    ''' |      |
    ''' |      +--&gt; Least Siginificant Bit
    ''' |
    ''' +---------&gt; Most Siginificant Bit
    ''' </code>
    ''' <code lang="vb">
    ''' Dim bsData As New BitStream(DataStream)
    ''' Dim BIT As Long
    ''' 
    ''' '
    ''' ' Using Default BitOrder: MSB
    ''' '
    ''' BIT = bsData.GetBits(2)   ' -- Returns  1 (01)
    ''' BIT = bsData.GetBits(4)   ' -- Returns  6 (0110)
    ''' 
    ''' '
    ''' ' Changing BitOrder to LSB
    ''' '
    ''' bsData.BitOrderRead = LSB
    ''' BIT = bsData.GetBits(2)   ' -- Returns  2 (10)
    ''' BIT = bsData.GetBits(4)   ' -- Returns 10 (1010)
    ''' 
    ''' '
    ''' ' Changing the order back to MSB
    ''' '
    ''' bsData.BitOrder = LSB
    ''' bsData.ByteOrder = MSB
    ''' BIT = bsData.GetBits(10) ' -- Returns 425 (01101001 01)
    ''' '                             Note that this call crossed
    ''' '                             the byte boundary and the
    ''' '                             BitStream object needed to
    ''' '                             read another byte from the stream.
    ''' 
    ''' bsData.ByteOrder = MSB
    ''' BIT = bsData.GetBits(10) ' -- Returns 425 (01101001 01)
    ''' '                             If no byte is read from the stream
    ''' '                             changing the order of how the bytes
    ''' '                             are taken from the strem does not
    ''' '                             change the bit buffer.
    ''' 
    ''' BIT = bsData.GetBits(18) ' -- Returns 57769 (11110000 01101010 01)
    ''' '                             Note that the third byte read
    ''' '                             came forward once it is now the
    ''' '                             Most Significant Byte.
    ''' </code>
    ''' </example>
    Public Property BitOrder() As ReadOrder
        Get
            Return __BitOrder
        End Get
        Set(ByVal Value As ReadOrder)
            __BitOrder = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the how the bytes read are from the original stream. 
    ''' </summary>
    ''' <remarks>
    ''' When setted to MSB all calls to functions that retrieve bits from the 
    ''' stream returns first the Most Significant Bits.
    ''' <p />
    ''' Likewise, when setted to LSB all calls returns first the Least Significant 
    ''' Bits.
    ''' <p />
    ''' Becausethe BitStream object cannot read less than a byte from the original 
    ''' stream be aware that a reading bits from the stream might cross the byte 
    ''' boundary. To ensure the most correct answer possible make sure to set 
    ''' both BitOrder and ByteOrder properties to avoid unexpected behaviors.
    ''' <p />
    ''' By default the ByteOrder is setted to LSB.
    ''' </remarks>
    ''' <example>
    ''' <code title="The example below considers this stream as its source">
    ''' 01101010 01010101 11110000 10101010
    ''' |      |
    ''' |      +--&gt; Least Siginificant Bit
    ''' |
    ''' +---------&gt; Most Siginificant Bit
    ''' </code>
    ''' <code lang="vb">
    ''' Dim bsData As New BitStream(DataStream)
    ''' Dim BIT As Long
    ''' 
    ''' '
    ''' ' Using Default BitOrder: MSB
    ''' '
    ''' BIT = bsData.GetBits(2)   ' -- Returns  1 (01)
    ''' BIT = bsData.GetBits(4)   ' -- Returns  6 (0110)
    ''' 
    ''' '
    ''' ' Changing BitOrder to LSB
    ''' '
    ''' bsData.BitOrderRead = LSB
    ''' BIT = bsData.GetBits(2)   ' -- Returns  2 (10)
    ''' BIT = bsData.GetBits(4)   ' -- Returns 10 (1010)
    ''' 
    ''' '
    ''' ' Changing the order back to MSB
    ''' '
    ''' bsData.BitOrder = LSB
    ''' bsData.ByteOrder = MSB
    ''' BIT = bsData.GetBits(10) ' -- Returns 425 (01101001 01)
    ''' '                             Note that this call crossed
    ''' '                             the byte boundary and the
    ''' '                             BitStream object needed to
    ''' '                             read another byte from the stream.
    ''' 
    ''' bsData.ByteOrder = MSB
    ''' BIT = bsData.GetBits(10) ' -- Returns 425 (01101001 01)
    ''' '                             Changing the order of how the bytes
    ''' '                             are taken from the strem does not
    ''' '                             change the bit buffer.
    ''' 
    ''' BIT = bsData.GetBits(18) ' -- Returns 57769 (11110000 01101010 01)
    ''' '                             Note that the third byte read
    ''' '                             came forward once it is now the
    ''' '                             Most Significant Byte.
    ''' </code>
    ''' </example>
    Public Property ByteOrder() As ReadOrder
        Get
            Return __ByteOrder
        End Get
        Set(ByVal Value As ReadOrder)
            __ByteOrder = Value
        End Set
    End Property

    ''' <summary>
    ''' Number of bits in the bit buffer.
    ''' </summary>
    Public ReadOnly Property BitsInBuffer() As Byte
        Get
            Return __NumBits
        End Get
    End Property

    ''' <summary>
    ''' The original stream from where the bits are readen.
    ''' </summary>
    Public ReadOnly Property InnerStream() As System.IO.Stream
        Get
            Return __Stream
        End Get
    End Property

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Gets a sequence of bits, up to 64, from the stream into a bit buffer.
    ''' </summary>
    ''' <param name="NumBits">Number of bits to read.</param>
    ''' <exception cref="ArgumentOutOfRangeException">The argument <paramref name="NumBits" /> is out of range.</exception>
    ''' <exception cref="zlibVBNET.Exceptions.ReadPastEndException">An attempt to read after the end of the stream.</exception>
    ''' <returns>A <see cref="Long" /> that is the value of the bits read from the <see cref="zlibVBNET.BitStream" />.</returns>
    Public Function GetBits(ByVal NumBits As Byte) As Long
        If (NumBits < 0) OrElse (NumBits > 64) Then
            Throw New ArgumentOutOfRangeException("NumBits", NumBits, My.Resources.BitStream_NumBits_OutOfRange)
        End If

        Do While (__NumBits < NumBits)
            Dim intByte As Integer = GetByte(__Stream)
            If (intByte = (-1)) Then
                Throw New zlibVBNET.Exceptions.ReadPastEndException
            End If
            If (__ByteOrder = ReadOrder.LSB) Then
                __BitBuffer = __BitBuffer Or (CLng(intByte) << __NumBits)
            Else
                __BitBuffer = (__BitBuffer << 8) Or intByte
            End If
            __NumBits += 8
        Loop

        If (__BitOrder = ReadOrder.LSB) Then
            Return (__BitBuffer And zlibVBNET.Masks.BitMask(NumBits))
        Else
            Return (__BitBuffer >> (__NumBits - NumBits))
        End If
    End Function

    ''' <summary>
    ''' Removes an amount of bits from the bit buffer.
    ''' </summary>
    ''' <param name="NumBits">Number of bits to discart from the bit buffer.</param>
    ''' <exception cref="ArgumentOutOfRangeException">The argument <paramref name="NumBits" /> is out of range.</exception>
    Public Sub DumpBits(ByVal NumBits As Byte)
        If (NumBits < 0) OrElse (NumBits > 64) Then
            Throw New ArgumentOutOfRangeException("NumBits", NumBits, My.Resources.BitStream_NumBits_OutOfRange)
        End If

        '*
        '* Remove the bits from buffer
        '*
        If (__BitOrder = ReadOrder.LSB) Then
            __BitBuffer >>= NumBits
        Else
            __BitBuffer = (__BitBuffer And zlibVBNET.Masks.BitMask(__NumBits - NumBits))
        End If
        __NumBits -= NumBits

        If (__NumBits < 0) Then
            __NumBits = 0
        End If
    End Sub

    ''' <summary>
    ''' Reads a sequence of bits, up to 64, from the stream.
    ''' </summary>
    ''' <param name="NumBits">Number of bits to read.</param>
    ''' <remarks>
    ''' Similar to the GetBits function, but the ReadBits consume the bits from the buffer.
    ''' <p />
    ''' For instance:
    ''' <p />
    ''' Two consecutives calls of GetBits(4) would read a whole byte into the buffer.
    ''' <p />
    ''' Two consecutives calls of ReadBits(4) would consume a whole byte from the stream.
    ''' </remarks>
    ''' <returns>A <see cref="Long"/> that represents the value of the bits read from the stream.</returns>
    <DebuggerStepThrough()> _
    Public Function ReadBits(ByVal NumBits As Byte) As Long
        Dim intBits As Long
        intBits = GetBits(NumBits)
        DumpBits(NumBits)
        Return intBits
    End Function

#End Region

End Class
