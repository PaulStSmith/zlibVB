'**************************************************
' FILE:         ReadPastEndException.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Represents an attempt to read 
'       past the end of the data.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from Errors.vb
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Represents an attempt to read past the end of the data.
    ''' </summary>
    Public Class ReadPastEndException
        Inherits System.Exception

        ''' <summary>
        ''' Initializes an instance of the <see cref="ReadPastEndException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        <DebuggerStepThrough()> _
        Friend Sub New()
            MyBase.New(My.Resources.ReadPastEnd_Message)
        End Sub

    End Class

End Namespace
