'**************************************************
' FILE:         Masks.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Allow to retrieve a bit mask from 
'       either binary or hexa decimal codes.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from gzip.vb
'       Changed from Class to Module
'***************************************************

''' <summary>
''' Allow to retrieve a bit mask from either binary or hexa decimal codes.
''' </summary>
Public Module Masks

#Region " Public Shared Methods "

    ''' <summary>
    ''' Returns the maximum number allowed by the number of bits informed.
    ''' </summary>
    ''' <param name="NumBits">Number of bits that would be set.</param>
    ''' <remarks>
    '''	<example>
    ''' BitMask(3)  returns  7, because it's formed by 111 b.
    ''' <p />
    ''' BitMask(64) returns -1, because it's formed by 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 b.
    ''' </example>
    '''	<p />
    '''	<b>Note:</b> To be CLS-Compliant this method use the <see cref="Long" /> datatype, instead the <see cref="ULong" />.
    ''' Therefore it may return negative numbers due the fact how the numbers are stored in memory, as shown in the example.
    ''' </remarks>
    ''' <returns>The maximum number allowed by the number of bits informed.</returns>
    ''' <exception cref="ArgumentOutOfRangeException">The argument <paramref name="NumBits" /> is less than 0 or greater than 64.</exception>
    Public Function BitMask(ByVal NumBits As Integer) As Long
        If (NumBits < 0) OrElse (NumBits > 64) Then
            Throw New ArgumentOutOfRangeException("NumBits", NumBits, My.Resources.Mask_BitMask_OutOfRangeException)
        End If

        Return ((Not 0UL) >> (64 - NumBits))
    End Function

    ''' <summary>
    ''' Returns the string representation of the bits of a given hexadecimal value.
    ''' </summary>
    ''' <param name="HexCode">Hexadecimal code to retrieve it's bit representation.</param>
    ''' <returns>A <see cref="String" /> containing the human-readable representation of the bits set on the hexadecimal value.</returns>
    ''' <remarks>
    ''' The character "0" means a bit 0 and the character "1" a bit 1.
    ''' The most significant bit (MSB) is represented on the left.
    ''' <p />
    ''' For instance HexCode(5) returns the string "0101".
    ''' </remarks>
    ''' <exception cref="ArgumentOutOfRangeException">The argument <paramref name="HexCode" /> is less than 0 or greater than 15.</exception>
    ''' <overloads>Returns a string representation of the bits in a given hexadecimal value, char or string.</overloads>
    Public Function HexMask(ByVal HexCode As Integer) As String
        If (HexCode < 0) OrElse (HexCode > 15) Then
            Throw New ArgumentOutOfRangeException("HexCode", HexCode, My.Resources.Mask_HexMask_OutOfRangeException)
        End If

        Static Mask() As String = {"0000", "0001", "0010", "0011", _
                                   "0100", "0101", "0110", "0111", _
                                   "1000", "1001", "1010", "1011", _
                                   "1100", "1101", "1110", "1111"}

        Return Mask(HexCode)
    End Function

    ''' <summary>
    ''' Returns the string representation of the bits of a given hexadecimal char.
    ''' </summary>
    ''' <param name="HexChar">The hexadecimal char to retrieve its bit representation.</param>
    ''' <returns>A <see cref="String" /> containing the human-readable representation of the bits set on the hexadecimal value.</returns>
    ''' <remarks>
    ''' The character "0" means a bit 0 and the character "1" a bit 1.
    ''' The most significant bit (MSB) is represented on the left.
    ''' <p />
    ''' For instance HexCode("A"c) returns the string "1010".
    ''' </remarks>
    ''' <exception cref="ArgumentOutOfRangeException">The argument <paramref name="HexChar" /> is not a valid hexadecimal character [0-9A-F].</exception>
    Public Function HexMask(ByVal HexChar As Char) As String
        Dim iPos As Integer = ("0123456789abcdef").IndexOf(Char.ToLowerInvariant(HexChar))
        If (iPos = (-1)) Then
            Throw New ArgumentOutOfRangeException("HexChar", HexChar, My.Resources.Mask_HexMask_Char_OutOfRangeException)
        End If

        Return HexMask(CByte(iPos))
    End Function

    ''' <summary>
    ''' Returns the string representation of the bits of a given hexadecimal string.
    ''' </summary>
    ''' <param name="HexString">Hexadecimal string to retrieve it's bit representation.</param>
    ''' <returns>A <see cref="String"/> containing the human-readable representation of the bits set on the hexadecimal value.</returns>
    ''' <remarks>
    ''' The character "0" means a bit 0 and the character "1" a bit 1.
    ''' The most significant bit (MSB) is represented on the left.
    ''' <p />
    ''' For instance HexCode("AB") returns the string "10101011".
    ''' </remarks>
    Public Function HexMask(ByVal HexString As String) As String
        Dim sb As New System.Text.StringBuilder
        For Each strChar As Char In HexString.ToCharArray
            sb.Append(HexMask(strChar))
        Next
        Return sb.ToString()
    End Function

#End Region

End Module
