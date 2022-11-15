'**************************************************
' FILE:         CompressedFile.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Enumerates all the implemented compression methods.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from zlibVBNET.vb
'***************************************************

Namespace GZIP

    ''' <summary>
    ''' Enumerates all the implemented compression methods.
    ''' </summary>
    Public Enum CompressionMethod As Integer
        ''' <summary>
        ''' The compression method is unknown
        ''' </summary>
        Unknown = (-1)

        ''' <summary>
        ''' The data was stored without compression
        ''' </summary>
        Stored = 0

        ''' <summary>
        ''' The data was stored using the LZW algorithm.
        ''' </summary>
        Compressed = 1

        ''' <summary>
        ''' The data was packed.
        ''' </summary>
        Packed = 2

        ''' <summary>
        ''' The data was stored using the LZH algorithm.
        ''' </summary>
        LZH = 3

        ''' <summary>
        ''' The data was stored using the Deflate algorithm.
        ''' </summary>
        ''' <remarks></remarks>
        Deflated = 8
    End Enum

End Namespace
