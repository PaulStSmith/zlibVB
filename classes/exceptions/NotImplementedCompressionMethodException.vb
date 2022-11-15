'**************************************************
' FILE:         NotImplementedCompressionMethodException.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Occurs when there is an attempt to 
'       decompress with an unimplemented 
'       compression method.
'
' MODIFICATION HISTORY:
' 01    2005.OCT.05
'       Paulo Santos
'       Extracted from Errors.vb
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Occurs when there is an attempt to decompress with an unimplemented compression method.
    ''' </summary>
    Public Class NotImplementedCompressionMethodException
        Inherits System.Exception

        Private __CompressionMethod As GZIP.CompressionMethod = GZIP.CompressionMethod.Unknown

        ''' <summary>
        ''' Initializes an instance of the <see cref="NotImplementedCompressionMethodException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        <DebuggerStepThrough()> _
        Friend Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="NotImplementedCompressionMethodException" /> class.
        ''' </summary>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal Message As String)
            MyBase.New(Message)
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="NotImplementedCompressionMethodException" /> class.
        ''' </summary>
        ''' <param name="compressionMethod">One of the <see cref="CompressionMethod"/> values that indicates the not implemented compression method.</param>
        Friend Sub New(ByVal compressionMethod As GZIP.CompressionMethod)
            MyBase.New()
            __CompressionMethod = compressionMethod
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="NotImplementedCompressionMethodException" /> class.
        ''' </summary>
        ''' <param name="compressionMethod">One of the <see cref="CompressionMethod"/> values that indicates the not implemented compression method.</param>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        Friend Sub New(ByVal compressionMethod As GZIP.CompressionMethod, ByVal message As String)
            MyBase.New(message)
            __CompressionMethod = compressionMethod
        End Sub

        ''' <summary>
        ''' Gets the not implemented compression method.
        ''' </summary>
        ''' <value>One of the <see cref="CompressionMethod"/> values that indicates the not implemented compression method.</value>
        Public ReadOnly Property CompressionMethod() As GZIP.CompressionMethod
            Get
                Return __CompressionMethod
            End Get
        End Property

    End Class

End Namespace

