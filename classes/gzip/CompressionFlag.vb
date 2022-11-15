'**************************************************
' FILE:         CompressionFlag.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Enumerates all the known compression flags.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from zlibVBNET.vb
'***************************************************

Imports System.ComponentModel

Namespace GZIP

    ''' <summary>
    ''' Enumerates all the known compression flags.
    ''' </summary>
    <Flags()> _
    Public Enum CompressionFlags As Integer
        ''' <summary>
        ''' Identifies that the compressed file probably ASCII text.
        ''' </summary>
        AscII = &H1

        ''' <summary>
        ''' Identifies that the stream is a continuation of a previous part.
        ''' </summary>
        Continuation = &H2

        ''' <summary>
        ''' Identifies that the extra field is present.
        ''' </summary>
        ExtraField = &H4

        ''' <summary>
        ''' Identifies that the original file name is present.
        ''' </summary>
        FileName = &H8

        ''' <summary>
        ''' Identifies that there are file comments in the data stream.
        ''' </summary>
        Comment = &H10

        ''' <summary>
        ''' Identifies that the data stream is encrypted.
        ''' </summary>
        Encrypted = &H20

        ''' <summary>
        ''' Reserved for future extensions.
        ''' </summary>
        Reserved = &HC0
    End Enum

End Namespace
