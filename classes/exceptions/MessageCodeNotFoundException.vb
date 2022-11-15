'**************************************************
' FILE:         MessageCodeNotFoundException.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Indicates that a Message Code was not 
'       found on the Custom Alphabet provided 
'       for the Huffman Tree.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from Errors.vb
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Indicates that a Message Code was not found on the Custom Alphabet provided for the Huffman Tree.
    ''' </summary>
    Public Class MessageCodeNotFoundException
        Inherits System.Exception

        ''' <summary>
        ''' Initializes an instance of the <see cref="MessageCodeNotFoundException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        <DebuggerStepThrough()> _
        Friend Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="MessageCodeNotFoundException" /> class.
        ''' </summary>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub

    End Class

End Namespace
