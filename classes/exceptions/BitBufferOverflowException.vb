'**************************************************
' FILE:         BitBufferOverflowException.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Indicates an overflow on the bit buffer.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from Errors.vb
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Indicates an overflow on the bit buffer.
    ''' </summary>
    Public Class BitBufferOverflowException
        Inherits System.Exception

        ''' <summary>
        ''' Initializes an instance of the <see cref="BitBufferOverflowException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        <DebuggerStepThrough()> _
        Friend Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="BitBufferOverflowException" /> class.
        ''' </summary>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub

    End Class

End Namespace
