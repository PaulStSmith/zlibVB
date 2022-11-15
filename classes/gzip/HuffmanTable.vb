'**************************************************
' FILE:         gzip.vb
' AUTHOR:       Paulo Santos
' CREATED:      2005.JAN.23
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       GZIP classes
'
' MODIFICATION HISTORY:
' 01    2005.JAN.23
'       Paulo Santos
'       Created the namespace GZIP
'---------------------------------------------------
' This library implements the following algoritms:
'
' * GZIP (de)compression
'---------------------------------------------------
' GZIP is Copyright © 1992-1993 Jean-loup Gailly
'***************************************************

Namespace GZIP

    ''' <summary>
    ''' Represents a Huffman Table as used by the GZIP algorithm.
    ''' </summary>
    Friend Class HuffmanTable

        ''' <summary>
        ''' Enumerates how the <see cref="HuffmanTable"/> object generates the Huffman Codes
        ''' </summary>
        Friend Enum BitOrder
            ''' <summary>
            ''' The bit order is normal. (MSB First).
            ''' </summary>
            Normal = 0

            ''' <summary>
            ''' The bit order is reversed. (LSB First).
            ''' </summary>
            Reverse
        End Enum

#Region " Classes "

        ''' <summary>
        ''' Represents a decoded value.
        ''' </summary>
        Public Class DecodedValue

            Private __NumBits As Integer
            Private __ExtraBits As Integer
            Private __Simple As Boolean
            Private __Value As Integer
            Private __Code As Integer
            Private __DecodeTable As DecodeTable

            ''' <summary>
            ''' Initializes an instance of the <see cref="DecodedValue" /> class.
            ''' This is the default constructor for this class.
            ''' </summary>
            Friend Sub New()
            End Sub

            ''' <summary>
            ''' Number of bits of this value.
            ''' </summary>
            ''' <returns>Number of bits of this value.</returns>
            Public Property NumBits() As Integer
                Get
                    Return __NumBits
                End Get
                Set(ByVal Value As Integer)
                    __NumBits = Value
                End Set
            End Property

            ''' <summary>
            ''' Number of extra bits to be read from the stream.
            ''' </summary>
            ''' <returns>Number of extra bits to be read from the stream.</returns>
            Public Property ExtraBits() As Integer
                Get
                    Return __ExtraBits
                End Get
                Set(ByVal Value As Integer)
                    __ExtraBits = Value
                End Set
            End Property

            ''' <summary>
            ''' A flag that indicate if the value is a literal (thus a simple value), or
            ''' contais more information about how to decode the value.
            ''' </summary>
            ''' <returns>A boolean object that indicate if the value is a literal or not.</returns>
            Public Property Simple() As Boolean
                Get
                    Return __Simple
                End Get
                Set(ByVal Value As Boolean)
                    __Simple = Value
                End Set
            End Property

            ''' <summary>
            ''' The actual value of the decoded value.
            ''' </summary>
            ''' <returns>A number that represents the actual value of the decoded value.</returns>
            Public Property Value() As Integer
                Get
                    Return __Value
                End Get
                Set(ByVal Value As Integer)
                    __Value = Value
                End Set
            End Property

            ''' <summary>
            ''' A nested GZIP decode table.
            ''' </summary>
            ''' <returns>A GZIP.DecodeTable class.</returns>
            Public Property DecodeTable() As DecodeTable
                Get
                    Return __DecodeTable
                End Get
                Set(ByVal Value As DecodeTable)
                    __DecodeTable = Value
                End Set
            End Property

        End Class

        ''' <summary>
        ''' Represents a decoder lookup table.
        ''' </summary>
        <System.ComponentModel.DefaultProperty("Item")> _
        Public Class DecodeTable
            Inherits System.Collections.Specialized.NameObjectCollectionBase

            ''' <summary>
            ''' Initializes an instance of the <see cref="DecodeTable" /> class.
            ''' This is the default constructor for this class.
            ''' </summary>
            Friend Sub New()
                MyBase.New()
            End Sub

            ''' <summary>
            ''' Adds a <see cref="DecodedValue"/> object into the decode table and assign the <paramref name="Code"/> to this object.
            ''' </summary>
            ''' <param name="code">A number that represents the Huffman Code of the value.</param>
            ''' <param name="value">A <see cref="DecodedValue"/> to be assigned to the Huffman Code.</param>
            Public Sub Add(ByVal code As Integer, ByVal value As DecodedValue)
                MyBase.BaseAdd(Hex(code), value)
            End Sub

            ''' <summary>
            ''' Remove the informed code from the Decode Table.
            ''' </summary>
            ''' <param name="Code">A number that represents the Huffman Code to be removed.</param>
            Public Sub Remove(ByVal code As Integer)
                MyBase.BaseRemove(Hex(code))
            End Sub

            ''' <summary>
            ''' Sets, or adds, a gzipDecodedValue object into the Decode Table assigning it to its Huffman Code.
            ''' </summary>
            ''' <param name="Code">A number that represents the Huffman Code of the value.</param>
            ''' <param name="Value">A gzipDecodedValue to be assigned to the Huffman Code.</param>
            Public Sub [Set](ByVal code As Integer, ByVal value As DecodedValue)
                MyBase.BaseSet(Hex(code), value)
            End Sub

            ''' <summary>
            ''' Retrieves a gzipDecodedValue object associated with the Huffman Code.
            ''' </summary>
            ''' <param name="Code">A number that represents the Huffman Code of the value.</param>
            ''' <returns>A gzipDecodedValue object associated with the Huffman Code.</returns>
            Default Public Property Item(ByVal code As Integer) As DecodedValue
                Get
                    Return MyBase.BaseGet(Hex(code))
                End Get
                Set(ByVal value As DecodedValue)
                    MyBase.BaseSet(Hex(code), value)
                End Set
            End Property

            ''' <summary>
            ''' Gets all keys.
            ''' </summary>
            ''' <value>All keys.</value>
            Public ReadOnly Property AllKeys() As String()
                Get
                    Return MyBase.BaseGetAllKeys()
                End Get
            End Property

            ''' <summary>
            ''' Gets all values.
            ''' </summary>
            ''' <value>All values.</value>
            Public ReadOnly Property AllValues() As DecodedValue()
                Get
                    Return MyBase.BaseGetAllValues(GetType(DecodedValue))
                End Get
            End Property

#Region " DEBUG "
#If DEBUG Then
            ''' <summary>
            ''' Gets the non null.
            ''' </summary>
            ''' <value>The non null.</value>
            Public ReadOnly Property NonNullKeys() As String
                Get
                    Dim sb As New System.Text.StringBuilder()
                    For Each strKey As String In MyBase.BaseGetAllKeys
                        If (Not MyBase.BaseGet(strKey) Is Nothing) Then
                            sb.Append(", " & Val("&H" & strKey))
                        End If
                    Next
                    Return sb.ToString().Substring(2)
                End Get
            End Property

            ''' <summary>
            ''' Gets the list values.
            ''' </summary>
            ''' <value>The list values.</value>
            Public ReadOnly Property ListValues() As String()
                Get
                    Dim lValues As New System.Collections.Generic.List(Of String)
                    For Each strKey As String In MyBase.BaseGetAllKeys
                        If (MyBase.BaseGet(strKey) Is Nothing) Then
                            lValues.Add(zlibVBNET.Masks.HexMask(strKey.PadLeft(2, "0"c)) & " : " & "Nothing")
                        Else
                            lValues.Add(Right(zlibVBNET.Masks.HexMask(strKey.PadLeft(4, "0"c)), MyBase.BaseGet(strKey).NumBits) & " : " & IfNull(MyBase.BaseGet(strKey).Value, "Nothing"))
                        End If
                    Next
                    Return lValues.ToArray()
                End Get
            End Property
#End If

#End Region

        End Class

#End Region

        Private __BitStream As zlibVBNET.BitStream
        Private __LengthCodeArray() As Integer
        Private __BaseValuesArray() As Integer
        Private __ExtraBitsArray() As Integer
        Private __MinLookupBits As Integer
        Private __MaxLookupBits As Byte
        Private __CodeCount As Integer
        Private __NumberSimpleCodes As Integer
        Private __BitCount As Integer
        Private __DecodeTable As DecodeTable

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        Public Sub New()
            __DecodeTable = New DecodeTable
            __LengthCodeArray = Nothing
            __BaseValuesArray = Nothing
            __ExtraBitsArray = Nothing
            __CodeCount = (-1)
            __NumberSimpleCodes = (-1)
            __MinLookupBits = 1
            __MaxLookupBits = 8
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' </summary>
        ''' <param name="LengthCodeArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Huffman Length Codes that will be used to generate the Huffman Codes.</param>
        Public Sub New(ByVal LengthCodeArray() As Integer)
            Me.New()
            __LengthCodeArray = LengthCodeArray
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' </summary>
        ''' <param name="LengthCodeArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Huffman Length Codes that will be used to generate the Huffman Codes.</param>
        ''' <param name="BaseValuesArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Values to be matched with the Huffman Codes generated.</param>
        Public Sub New(ByVal LengthCodeArray() As Integer, _
                       ByVal BaseValuesArray() As Integer)
            Me.New(LengthCodeArray)
            __BaseValuesArray = BaseValuesArray
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' </summary>
        ''' <param name="LengthCodeArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Huffman Length Codes that will be used to generate the Huffman Codes.</param>
        ''' <param name="BaseValuesArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Values to be matched with the Huffman Codes generated.</param>
        ''' <param name="ExtraBitsArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Extra Bits for the Deflate Algotithm.</param>
        Public Sub New(ByVal LengthCodeArray() As Integer, _
                       ByVal BaseValuesArray() As Integer, _
                       ByVal ExtraBitsArray() As Integer)
            Me.New(LengthCodeArray, BaseValuesArray)
            __ExtraBitsArray = ExtraBitsArray
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' </summary>
        ''' <param name="LengthCodeArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Huffman Length Codes that will be used to generate the Huffman Codes.</param>
        ''' <param name="BaseValuesArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Values to be matched with the Huffman Codes generated.</param>
        ''' <param name="ExtraBitsArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Extra Bits for the Deflate Algotithm.</param>
        ''' <param name="MaxLookupBits">A byte that represents the number of bits to be searched in the stream in order to find a positive match with the Huffman Codes generated.</param>
        Public Sub New(ByVal LengthCodeArray() As Integer, _
                       ByVal BaseValuesArray() As Integer, _
                       ByVal ExtraBitsArray() As Integer, _
                       ByVal MaxLookupBits As Byte)
            Me.New(LengthCodeArray, BaseValuesArray, ExtraBitsArray)
            __MaxLookupBits = MaxLookupBits
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="HuffmanTable" /> class.
        ''' </summary>
        ''' <param name="LengthCodeArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Huffman Length Codes that will be used to generate the Huffman Codes.</param>
        ''' <param name="BaseValuesArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Values to be matched with the Huffman Codes generated.</param>
        ''' <param name="ExtraBitsArray">An <see cref="Array"/> of <see cref="Integer"/> containing all the Extra Bits for the Deflate Algotithm.</param>
        ''' <param name="MaxLookupBits">A byte that represents the number of bits to be searched in the stream in order to find a positive match with the Huffman Codes generated.</param>
        ''' <param name="CodeCount">Number of the valid codes in the LenghtCodeArray.</param>
        ''' <param name="NumberSimpleCodes">Number of simple codes int the BaseValuesArray.</param>
        Public Sub New(ByVal LengthCodeArray() As Integer, _
                       ByVal BaseValuesArray() As Integer, _
                       ByVal ExtraBitsArray() As Integer, _
                       ByVal MaxLookupBits As Byte, _
                       ByVal CodeCount As Integer, _
                       ByVal NumberSimpleCodes As Integer)
            Me.New(LengthCodeArray, BaseValuesArray, ExtraBitsArray, MaxLookupBits)
            __CodeCount = CodeCount
            __NumberSimpleCodes = NumberSimpleCodes
        End Sub

        ''' <summary>
        ''' Represents a stream of bits that the <see cref="DecodeTable"/> class will 
        ''' use to decode the compressed data into meaningful data.
        ''' </summary>
        ''' <returns>A <see cref="zlibVBNET.BitStream"/> object that represents a stream of bits.</returns>
        Public Property BitStream() As zlibVBNET.BitStream
            Get
                Return __BitStream
            End Get
            Set(ByVal Value As zlibVBNET.BitStream)
                __BitStream = Value
            End Set
        End Property

        ''' <summary>
        ''' An <see cref="Array"/> of <see cref="Integer"/> containing all the code lengths that was used to encode the data.
        ''' </summary>
        Public Property LengthCodeArray() As Integer()
            Get
                Return __LengthCodeArray
            End Get
            Set(ByVal Value As Integer())
                __LengthCodeArray = Value
            End Set
        End Property

        ''' <summary>
        ''' An <see cref="Array"/> of <see cref="Integer"/> containing all the values represented by the Huffman Codes generated.
        ''' </summary>
        Public Property BaseValuesArray() As Integer()
            Get
                Return __BaseValuesArray
            End Get
            Set(ByVal Value As Integer())
                __BaseValuesArray = Value
            End Set
        End Property

        ''' <summary>
        ''' An <see cref="Array"/> of <see cref="Integer"/> containing all the extra bits for each value in the BaseValueArray.
        ''' </summary>
        Public Property ExtraBitsArray() As Integer()
            Get
                Return __ExtraBitsArray
            End Get
            Set(ByVal Value As Integer())
                __ExtraBitsArray = Value
            End Set
        End Property

        ''' <summary>
        ''' The maximum number of bits to be searched in the stream in order to find a positive match with the Huffman Codes generated.
        ''' </summary>
        Public Property MaxLookupBits() As Byte
            Get
                Return __MaxLookupBits
            End Get
            Set(ByVal Value As Byte)
                __MaxLookupBits = Value
            End Set
        End Property

        ''' <summary>
        ''' The minimum number of bits to be searched in the stream in order to find a positive match with the Huffman Codes generated.
        ''' </summary>
        Public Property MinLookupBits() As Byte
            Get
                Return __MinLookupBits
            End Get
            Set(ByVal Value As Byte)
                __MinLookupBits = Value
            End Set
        End Property

        ''' <summary>
        ''' Represents the number of valid codes in the LengthCodeArray.
        ''' </summary>
        Public Property CodeCount() As Integer
            Get
                Return __CodeCount
            End Get
            Set(ByVal Value As Integer)
                __CodeCount = Value
            End Set
        End Property

        ''' <summary>
        ''' Represents the number of simple codes (literal bytes) in the BaseValuesArray.
        ''' </summary>
        Public Property NumberSimpleCodes() As Integer
            Get
                Return __NumberSimpleCodes
            End Get
            Set(ByVal Value As Integer)
                __NumberSimpleCodes = Value
            End Set
        End Property

        ''' <summary>
        ''' The number of bits that this instance of <see cref="DecodeTable"/> can decode from de bit stream.
        ''' </summary>
        Public ReadOnly Property BitCount() As Integer
            Get
                Return __BitCount
            End Get
        End Property

        ''' <summary>
        ''' A GZIP.DecodeTable instance with all the codes generated.
        ''' </summary>
        Public ReadOnly Property DecoderTable() As DecodeTable
            Get
                Return __DecodeTable
            End Get
        End Property

        ''' <summary>
        ''' Generates the Huffman Code Table.
        ''' </summary>
        ''' <param name="BitOrder">
        ''' </param>
        ''' <exception cref="InvalidOperationException"><see cref="HuffmanTable.LengthCodeArray"/> is null (Nothing in Visial Basic) or is empty.</exception>
        ''' <exception cref="zlibVBNET.Exceptions.DecompressionException">The compressed data is invalid.</exception>
        Friend Sub GenerateTable(Optional ByVal BitOrder As BitOrder = zlibVBNET.GZIP.HuffmanTable.BitOrder.Normal)

            '*
            '* Check the valid input
            '*
            If (__LengthCodeArray Is Nothing) OrElse (__LengthCodeArray.Length = 0) Then
                Throw New InvalidOperationException(My.Resources.HuffmanTable_GenerateTable_LengthCodeArrayIsNull)
            End If

            Dim aCodes() As Integer = {}                ' <-- The Codes generated
            Dim aOffsets() As Integer = {}              ' <-- The next available code for each code length
            Dim aBitLengthCount() As Integer = {}       ' <-- Counts the occorrences of a given code length

            Dim intCode As Integer = 0
            Dim oValue As DecodedValue
            Dim intMinCodeLength As Integer = Integer.MaxValue
            Dim intMaxCodeLength As Integer = Integer.MinValue
            Dim intDummyCodes As Integer = 0
            Dim oTableDecode As DecodeTable

            '*
            '* Count how often each Length Code appears in the LengthCodeArray
            '*
            If (__CodeCount = (-1)) Then
                __CodeCount = __LengthCodeArray.Length
            End If

            For Each intLength As Integer In __LengthCodeArray
                Dim intCodeCount As Integer

                If (intLength >= aBitLengthCount.Length) Then
                    '*
                    '* Adjusts the aBitLengthCount Array
                    '*
                    Array.Resize(aBitLengthCount, intLength)
                End If

                aBitLengthCount(intLength) += 1         ' <-- Increments the Bit Length Code Counter
                intCodeCount += 1                       ' <-- Increments the Code Counter
                If (intCodeCount = __CodeCount) Then    ' <-- Check for the informed code count
                    Exit For
                End If
            Next

            '*
            '* Calculates the Minimum and Maximum Length Codes
            '*
            For intCount As Integer = 1 To aBitLengthCount.Length - 1  ' <-- Starts at 1 because no code has 0 length
                If (intMinCodeLength = Integer.MaxValue) AndAlso CBool(aBitLengthCount(intCount)) Then
                    intMinCodeLength = intCount
                End If
                If (intMaxCodeLength = Integer.MinValue) AndAlso CBool(aBitLengthCount(aBitLengthCount.Length - intCount)) Then
                    intMaxCodeLength = (aBitLengthCount.Length - intCount)
                End If
                If (intMaxCodeLength <> Integer.MinValue) AndAlso (intMinCodeLength <> Integer.MaxValue) Then
                    Exit For
                End If
            Next

            '*
            '* Check the Max Code Length against the MaxLookupBits
            '*
            If (intMinCodeLength > __MinLookupBits) Then
                __MinLookupBits = intMinCodeLength
            End If

            If (intMaxCodeLength > __MaxLookupBits) Then
                __MaxLookupBits = intMaxCodeLength
            End If
            __BitCount = __MaxLookupBits ' <-- BitCount represents the maximum bits on this instance

            '*
            '* Check if we have enough bits to generate all the codes needed.
            '*
            intDummyCodes = 1 << intMinCodeLength
            For intCount As Integer = intMinCodeLength To intMaxCodeLength
                intDummyCodes -= aBitLengthCount(intCount)
                If (intDummyCodes < 0) Then
                    Throw New zlibVBNET.Exceptions.DecompressionException(My.Resources.HuffmanTable_GenerateTable_MoreCodesThanBits)
                End If
                intDummyCodes <<= 1
            Next

            '*********************************************************
            '*         The following code was written based          *
            '*         on the algorithm provided by RFC1951          *
            '*-------------------------------------------------------*
            '*       REMARKS ON THE ENCODED GZIP HUFFMAN TREE        *
            '*                                                       *
            '* The RFC1951 states that the codes should be generated *
            '* with  this   algorithm, +-----------------------------*
            '* but  it  is  not  quite |  From the RFC1951           *
            '* clear  that  the  codes |  example code:              *
            '* are stored  with a  re- +-----------------------------*
            '* verse bir order.        |  Symbol Length Code Storage *
            '*                         |  ------ ------ ---- ------- *
            '* The MSB of the code  is |    A      3     010     010 *
            '* the LSB at the storage. |    B      3     011     110 *
            '* Therefore in  order  to |    C      3     100     001 *
            '* achieve  the    correct |    D      3     101     101 *
            '* codes to compare   with |    E      3     110     011 *
            '* the input  bit   stream |    F      2      00      00 *
            '* the   bits   must    be |    G      4    1110    0111 *
            '* reversed.               |    H      4    1111    1111 *
            '*********************************************************

            '*
            '* Calculates the Next Available Huffman code for each code length
            '*
            intCode = 0 ' <-- First Huffman Code
            Array.Resize(aOffsets, intMaxCodeLength)
            aBitLengthCount(0) = 0
            For intCount As Integer = 1 To intMaxCodeLength
                intCode = (intCode + aBitLengthCount(intCount - 1)) << 1
                aOffsets(intCount) = intCode
            Next

            '*
            '* Calculates the Huffman Code for each code length
            '*
            oTableDecode = New DecodeTable
            Array.Resize(aCodes, (__CodeCount - 1))
            For intCount As Integer = 0 To (__CodeCount - 1)
                Dim intLength As Integer = __LengthCodeArray(intCount)

                If (intLength <> 0) Then
                    If (BitOrder = BitOrder.Reverse) Then
                        aCodes(intCount) = ReverseBitOrder(aOffsets(intLength), intLength)
                    Else
                        aCodes(intCount) = aOffsets(intLength)
                    End If

                    oValue = New DecodedValue
                    oValue.NumBits = intLength
                    If (intCount < __NumberSimpleCodes) Then
                        oValue.Simple = True
                        oValue.ExtraBits = IIf(intCount < 256, 16, 15)
                        oValue.Value = intCount
                    Else
                        oValue.Simple = False
                        oValue.Value = __BaseValuesArray(intCount - __NumberSimpleCodes)
                        oValue.ExtraBits = __ExtraBitsArray(intCount - __NumberSimpleCodes)
                    End If

                    oTableDecode.Add(aCodes(intCount), oValue)
                    aOffsets(intLength) += 1
                End If
            Next

            __DecodeTable = oTableDecode

        End Sub

    End Class

End Namespace