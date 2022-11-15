'**************************************************
' FILE      : UnknownCompressionMethodException.vb
' AUTHOR    : Paulo.Santos
' CREATION  : 10/6/2007 1:57:47 PM
' COPYRIGHT : Copyright © 2007
'             PJ on Development
'             All Rights Reserved.
'
' Description:
'       Represents an error while decompressing a GZIP stream the decompressor encounters an unknown compression method.
'
' Change log:
' 0.1   10/6/2007 1:57:47 PM
'       Paulo.Santos
'       Created.
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Represents an error while decompressing a GZIP stream the decompressor encounters an unknown compression method.
    ''' </summary>
    Public Class UnknownCompressionMethodException
        Inherits Exception

        ''' <summary>
        ''' Initializes an instance of the <see cref="UnknownCompressionMethodException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        Friend Sub New()
            MyBase.New(My.Resources.UnknownCompressionMethod_Message)
        End Sub

    End Class

End Namespace

