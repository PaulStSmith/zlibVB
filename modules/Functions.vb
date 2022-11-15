'**************************************************
' FILE:         Functions
' AUTHOR:       Paulo Santos
' CREATED:      2005.JAN.14
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       zlibVB.NET miscellaneous support functions
'
' MODIFICATION HISTORY:
' 01    2005.JAN.14
'       Paulo Santos
'       Initial Version
'***************************************************

''' <summary>
''' Generic functions for the zlibVB.NET Assembly.
''' </summary>
Public Module Functions

    Private __Table_CRC32 As Integer() = { _
                &H0, &H77073096, &HEE0E612C, &H990951BA, &H76DC419, _
                &H706AF48F, &HE963A535, &H9E6495A3, &HEDB8832, &H79DCB8A4, _
                &HE0D5E91E, &H97D2D988, &H9B64C2B, &H7EB17CBD, &HE7B82D07, _
                &H90BF1D91, &H1DB71064, &H6AB020F2, &HF3B97148, &H84BE41DE, _
                &H1ADAD47D, &H6DDDE4EB, &HF4D4B551, &H83D385C7, &H136C9856, _
                &H646BA8C0, &HFD62F97A, &H8A65C9EC, &H14015C4F, &H63066CD9, _
                &HFA0F3D63, &H8D080DF5, &H3B6E20C8, &H4C69105E, &HD56041E4, _
                &HA2677172, &H3C03E4D1, &H4B04D447, &HD20D85FD, &HA50AB56B, _
                &H35B5A8FA, &H42B2986C, &HDBBBC9D6, &HACBCF940, &H32D86CE3, _
                &H45DF5C75, &HDCD60DCF, &HABD13D59, &H26D930AC, &H51DE003A, _
                &HC8D75180, &HBFD06116, &H21B4F4B5, &H56B3C423, &HCFBA9599, _
                &HB8BDA50F, &H2802B89E, &H5F058808, &HC60CD9B2, &HB10BE924, _
                &H2F6F7C87, &H58684C11, &HC1611DAB, &HB6662D3D, &H76DC4190, _
                &H1DB7106, &H98D220BC, &HEFD5102A, &H71B18589, &H6B6B51F, _
                &H9FBFE4A5, &HE8B8D433, &H7807C9A2, &HF00F934, &H9609A88E, _
                &HE10E9818, &H7F6A0DBB, &H86D3D2D, &H91646C97, &HE6635C01, _
                &H6B6B51F4, &H1C6C6162, &H856530D8, &HF262004E, &H6C0695ED, _
                &H1B01A57B, &H8208F4C1, &HF50FC457, &H65B0D9C6, &H12B7E950, _
                &H8BBEB8EA, &HFCB9887C, &H62DD1DDF, &H15DA2D49, &H8CD37CF3, _
                &HFBD44C65, &H4DB26158, &H3AB551CE, &HA3BC0074, &HD4BB30E2, _
                &H4ADFA541, &H3DD895D7, &HA4D1C46D, &HD3D6F4FB, &H4369E96A, _
                &H346ED9FC, &HAD678846, &HDA60B8D0, &H44042D73, &H33031DE5, _
                &HAA0A4C5F, &HDD0D7CC9, &H5005713C, &H270241AA, &HBE0B1010, _
                &HC90C2086, &H5768B525, &H206F85B3, &HB966D409, &HCE61E49F, _
                &H5EDEF90E, &H29D9C998, &HB0D09822, &HC7D7A8B4, &H59B33D17, _
                &H2EB40D81, &HB7BD5C3B, &HC0BA6CAD, &HEDB88320, &H9ABFB3B6, _
                &H3B6E20C, &H74B1D29A, &HEAD54739, &H9DD277AF, &H4DB2615, _
                &H73DC1683, &HE3630B12, &H94643B84, &HD6D6A3E, &H7A6A5AA8, _
                &HE40ECF0B, &H9309FF9D, &HA00AE27, &H7D079EB1, &HF00F9344, _
                &H8708A3D2, &H1E01F268, &H6906C2FE, &HF762575D, &H806567CB, _
                &H196C3671, &H6E6B06E7, &HFED41B76, &H89D32BE0, &H10DA7A5A, _
                &H67DD4ACC, &HF9B9DF6F, &H8EBEEFF9, &H17B7BE43, &H60B08ED5, _
                &HD6D6A3E8, &HA1D1937E, &H38D8C2C4, &H4FDFF252, &HD1BB67F1, _
                &HA6BC5767, &H3FB506DD, &H48B2364B, &HD80D2BDA, &HAF0A1B4C, _
                &H36034AF6, &H41047A60, &HDF60EFC3, &HA867DF55, &H316E8EEF, _
                &H4669BE79, &HCB61B38C, &HBC66831A, &H256FD2A0, &H5268E236, _
                &HCC0C7795, &HBB0B4703, &H220216B9, &H5505262F, &HC5BA3BBE, _
                &HB2BD0B28, &H2BB45A92, &H5CB36A04, &HC2D7FFA7, &HB5D0CF31, _
                &H2CD99E8B, &H5BDEAE1D, &H9B64C2B0, &HEC63F226, &H756AA39C, _
                &H26D930A, &H9C0906A9, &HEB0E363F, &H72076785, &H5005713, _
                &H95BF4A82, &HE2B87A14, &H7BB12BAE, &HCB61B38, &H92D28E9B, _
                &HE5D5BE0D, &H7CDCEFB7, &HBDBDF21, &H86D3D2D4, &HF1D4E242, _
                &H68DDB3F8, &H1FDA836E, &H81BE16CD, &HF6B9265B, &H6FB077E1, _
                &H18B74777, &H88085AE6, &HFF0F6A70, &H66063BCA, &H11010B5C, _
                &H8F659EFF, &HF862AE69, &H616BFFD3, &H166CCF45, &HA00AE278, _
                &HD70DD2EE, &H4E048354, &H3903B3C2, &HA7672661, &HD06016F7, _
                &H4969474D, &H3E6E77DB, &HAED16A4A, &HD9D65ADC, &H40DF0B66, _
                &H37D83BF0, &HA9BCAE53, &HDEBB9EC5, &H47B2CF7F, &H30B5FFE9, _
                &HBDBDF21C, &HCABAC28A, &H53B39330, &H24B4A3A6, &HBAD03605, _
                &HCDD70693, &H54DE5729, &H23D967BF, &HB3667A2E, &HC4614AB8, _
                &H5D681B02, &H2A6F2B94, &HB40BBE37, &HC30C8EA1, &H5A05DF1B, _
                &H2D02EF8D}

    ''' <summary>
    ''' Computes a CRC32 checksum of a byte array for each byte up to the specified length.
    ''' </summary>
    ''' <param name="ByteArray">A byte array to compute the CRC32.</param>
    ''' <param name="Length">The length of bytes to compute the CRC32.</param>
    ''' <returns>A number that represents the CRC32 checksum.</returns>
    ''' <remarks>
    ''' In order to compute the CRC32 we need first to generate a table for 
    ''' a byte-wise 32-bit CRC calculation on the polynomial:
    ''' <p />
    '''   x<sup>32</sup>+x<sup>26</sup>+x<sup>23</sup>+x<sup>22</sup>+x<sup>16</sup>+x<sup>12</sup>+x<sup>11</sup>+x<sup>10</sup>+x<sup>8</sup>+x<sup>7</sup>+x<sup>5</sup>+x<sup>4</sup>+x<sup>2</sup>+x+1.
    ''' <p />
    ''' Polynomials over GF(2) are represented in binary, one bit per coefficient,
    ''' with the lowest powers in the most significant bit.  Then adding polynomials
    ''' is just exclusive-or, and multiplying a polynomial by x is a right shift by
    ''' one.  If we call the above polynomial p, and represent a byte as the
    ''' polynomial q, also with the lowest power in the most significant bit (so the
    ''' byte 0xb1 is the polynomial x<sup>7</sup>+x<sup>3</sup>+x+1), then the CRC is (q*x<sup>32</sup>) mod p,
    ''' where a mod b means the remainder after dividing a by b.
    ''' <p />
    ''' This calculation is done using the shift-register method of multiplying and
    ''' taking the remainder.  The register is initialized to zero, and for each
    ''' incoming bit, x<sup>32</sup> is added mod p to the register if the bit is a one (where
    ''' x<sup>32</sup> mod p is p+x<sup>32</sup> = x<sup>26</sup>+...+1), and the register is multiplied mod p by
    ''' x (which is shifting right by one and adding x<sup>32</sup> mod p if the bit shifted
    ''' out is a one).  We start with the highest power (least significant bit) of
    ''' q and repeat for all eight bits of q.
    ''' <p />
    ''' The table is simply the CRC of all possible eight bit values.  This is all
    ''' the information needed to generate CRC's on data a byte at a time for all
    ''' combinations of CRC register values and incoming bytes.
    ''' </remarks>
    ''' <overloads>Calculates a CRC32 checksum of a byte array.</overloads>
    Public Function CRC32(ByVal ByteArray As Byte(), ByVal Length As Integer) As Integer

        Dim intCRC As Integer
        Static __CRC32 As Integer = &HFFFFFFFF ' <-- CRC32 Starts with this value

        '*
        '* Before we calculate the CRC32 we must fist prepare calling with nothing
        '*
        If (ByteArray Is Nothing) Then
            intCRC = &HFFFFFFFF
        Else
            intCRC = __CRC32
            If (Length) Then
                Dim intCount As Integer
                For intCount = Length - 1 To 0 Step -1
                    intCRC = __Table_CRC32((CShort(intCRC) Xor ByteArray(intCount)) And &HFF) Xor (intCRC \ &H100)
                Next
            End If
        End If

        __CRC32 = intCRC
        Return (__CRC32 Xor &HFFFFFFFF)

    End Function

    ''' <summary>
    ''' Computes a CRC32 checksum for the specified byte array.
    ''' </summary>
    ''' <param name="ByteArray">A byte array to compute the CRC32.</param>
    ''' <returns>A number that represents the CRC32 checksum.</returns>
    ''' <remarks>
    ''' In order to compute the CRC32 we need first to generate a table for 
    ''' a byte-wise 32-bit CRC calculation on the polynomial:
    ''' <p />
    '''   x<sup>32</sup>+x<sup>26</sup>+x<sup>23</sup>+x<sup>22</sup>+x<sup>16</sup>+x<sup>12</sup>+x<sup>11</sup>+x<sup>10</sup>+x<sup>8</sup>+x<sup>7</sup>+x<sup>5</sup>+x<sup>4</sup>+x<sup>2</sup>+x+1.
    ''' <p />
    ''' Polynomials over GF(2) are represented in binary, one bit per coefficient,
    ''' with the lowest powers in the most significant bit.  Then adding polynomials
    ''' is just exclusive-or, and multiplying a polynomial by x is a right shift by
    ''' one.  If we call the above polynomial p, and represent a byte as the
    ''' polynomial q, also with the lowest power in the most significant bit (so the
    ''' byte 0xb1 is the polynomial x<sup>7</sup>+x<sup>3</sup>+x+1), then the CRC is (q*x<sup>32</sup>) mod p,
    ''' where a mod b means the remainder after dividing a by b.
    ''' <p />
    ''' This calculation is done using the shift-register method of multiplying and
    ''' taking the remainder.  The register is initialized to zero, and for each
    ''' incoming bit, x<sup>32</sup> is added mod p to the register if the bit is a one (where
    ''' x<sup>32</sup> mod p is p+x<sup>32</sup> = x<sup>26</sup>+...+1), and the register is multiplied mod p by
    ''' x (which is shifting right by one and adding x<sup>32</sup> mod p if the bit shifted
    ''' out is a one).  We start with the highest power (least significant bit) of
    ''' q and repeat for all eight bits of q.
    ''' <p />
    ''' The table is simply the CRC of all possible eight bit values.  This is all
    ''' the information needed to generate CRC's on data a byte at a time for all
    ''' combinations of CRC register values and incoming bytes.
    ''' </remarks>
    Public Function CRC32(ByVal ByteArray As Byte()) As Integer
        Call CRC32(ByteArray, ByteArray.Length)
    End Function

    ''' <summary>
    ''' Get a Byte from a Stream.
    ''' </summary>
    ''' <param name="DataStream">Stream to read from.</param>
    ''' <returns>The byte read.</returns>
    ''' <exception cref="zlibVBNET.Exceptions.ReadPastEndException">An attempt to read past the end of the stream occurred.</exception>
    <DebuggerStepThrough()> _
    Public Function GetByte(ByVal DataStream As System.IO.Stream) As Byte

        Try
            Dim intByte As Integer
            intByte = DataStream.ReadByte
            If (intByte = (-1)) Then
                Throw New zlibVBNET.Exceptions.ReadPastEndException()
            End If
            Return intByte
        Catch
            Throw
        End Try

    End Function

    ''' <summary>
    ''' Returns <paramref name="DefaultValue"/> if <paramref name="Object"/> is <langword name="null" />.
    ''' </summary>
    ''' <param name="Object">Object to test.</param>
    ''' <param name="DefaultValue">Default value in case Object is <langword name="null" />.</param>
    ''' <returns>Either the Object or the Default Value, depending if object is <langword name="null" />.</returns>
    <DebuggerStepThrough()> _
    Public Function IfNull(ByVal [Object] As Object, ByVal DefaultValue As Object) As Object

        If ([Object] Is Nothing) Then
            Return DefaultValue
        Else
            Return [Object]
        End If

    End Function

    ''' <summary>
    ''' Performs shift and rotate for the number of bits requested.
    ''' </summary>
    ''' <remarks>
    ''' <code title="Example">
    ''' Number                Length   Bits    Result
    ''' 0x65  (    01100101)       8      4    0x56  (    01010110)
    ''' 0x356 (001101010110)       8      4    0x65  (    01100101)
    ''' 0x125 (000100100101)      12      4    0x251 (001001010001)
    ''' </code>
    ''' </remarks>
    ''' <param name="Number">The number to be rotate.</param>
    ''' <param name="Length">The Length in bits to be considered.</param>
    ''' <param name="Bits">The number of bits to perform the operation.</param>
    ''' <returns>A number that is the result of the shift and rotate operation.</returns>
    Public Function ShiftAndRotate(ByVal Number As Long, ByVal Length As Byte, ByVal Bits As Byte) As Long

        Dim lngAux As Long

        lngAux = Number And zlibVBNET.Masks.BitMask(Length)
        For intCount As Integer = 1 To Bits
            Dim lngCarry As Long = lngAux And 1
            lngAux >>= 1
            lngAux = lngAux Or (lngCarry << (Bits - 1))
        Next
        Return lngAux

    End Function

    ''' <summary>
    ''' Inverts the bit order of a number.
    ''' </summary>
    ''' <param name="Number">The number to have its bit order reversed.</param>
    ''' <param name="Length">The Length in bits to be considered.</param>
    ''' <returns>A number that is the number with the bits in reverse order.</returns>
    Public Function ReverseBitOrder(ByVal Number As Long, ByVal Length As Byte) As Long

        Dim lngAux As Long

        Number = Number And zlibVBNET.Masks.BitMask(Length)
        For intCount As Integer = 1 To Length
            Dim lngCarry As Long = Number And 1
            Number >>= 1
            lngAux = (lngAux << 1) Or lngCarry
        Next
        Return lngAux

    End Function

End Module