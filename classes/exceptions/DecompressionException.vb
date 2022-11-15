'**************************************************
' FILE:         DecompressionException.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Represents errors that occur during 
'       the evaluation of a GZIP file or stream.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from Errors.vb
'***************************************************

Namespace Exceptions

    ''' <summary>
    ''' Represents errors that occur during the evaluation of a GZIP file or stream.
    ''' </summary>
    Public Class DecompressionException
        Inherits System.Exception

        Private __Stream As System.IO.Stream

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        Friend Sub New()
            MyBase.New()
            __Stream = Nothing
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        Friend Sub New(ByVal Message As String)
            MyBase.New(Message)
            __Stream = Nothing
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="DataStream">The original stream that generated the error.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal DataStream As System.IO.Stream)
            MyBase.New()
            __Stream = DataStream
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="DataStream">The original stream that generated the error.</param>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal DataStream As System.IO.Stream, ByVal Message As String)
            MyBase.New(Message)
            __Stream = DataStream
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="DataStream">The original stream that generated the error.</param>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        ''' <param name="InnerException">The exception that is the cause of the current exception. If the innerException parameter is not <langword name="null" />, the current exception is raised in a catch block that handles the inner exception.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal DataStream As System.IO.Stream, ByVal Message As String, ByVal InnerException As System.Exception)
            MyBase.New(Message, InnerException)
            __Stream = DataStream
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="ByteArray">The original data that generated the error.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal ByteArray As Byte())
            MyBase.New()
            __Stream = New System.IO.MemoryStream(ByteArray)
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="ByteArray">The original data that generated the error.</param>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal ByteArray As Byte(), ByVal Message As String)
            MyBase.New(Message)
            __Stream = New System.IO.MemoryStream(ByteArray)
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="DecompressionException" /> class.
        ''' </summary>
        ''' <param name="ByteArray">The original data that generated the error.</param>
        ''' <param name="Message">An human-readable string describing the error more precisely.</param>
        ''' <param name="InnerException">The exception that is the cause of the current exception. If the innerException parameter is not <langword name="null" />, the current exception is raised in a catch block that handles the inner exception.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal ByteArray As Byte(), ByVal Message As String, ByVal InnerException As System.Exception)
            MyBase.New(Message, InnerException)
            __Stream = New System.IO.MemoryStream(ByteArray)
        End Sub

        ''' <summary>
        ''' The orginal <see cref="System.IO.Stream"/> that generated the error.
        ''' </summary>
        Public ReadOnly Property GzipStream() As System.IO.Stream
            <DebuggerHidden()> _
            Get
                Return __Stream
            End Get
        End Property

        ''' <summary>
        ''' The original data that generated the error.
        ''' </summary>
        Public ReadOnly Property GzipData() As Byte()
            <DebuggerHidden()> _
            Get
                Dim abytData As Byte() = {}

                __Stream.Read(abytData, 0, __Stream.Length)
                Return abytData
            End Get
        End Property

    End Class

End Namespace
