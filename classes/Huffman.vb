'**************************************************
' FILE:         Huffman
' AUTHOR:       Paulo Santos
' CREATED:      2005.JAN.13
' COPYRIGHT:    Copyright 2005 © Paulo Santos
'               All Rights Reserved.
' DESCRIPTION:
'       GZIP .NET classes library 
'
' MODIFICATION HISTORY:
' 01    2005.JAN.13
'       Paulo Santos
'       Finished the Huffman Encoding Class
'
' 02    2005.JAN.14
'       Paulo Santos
'       Added smmarized description of 
'       each component.
'---------------------------------------------------
' This library implements the Huffman Coding Tree
' algorithms. 
'***************************************************

Namespace Huffman

    ''' <summary>
    ''' Represents a root of a Huffman Tree.
    ''' </summary>
    Public Class Node
        Private __Value As Object

        Private __Code As Long
        Private __CodeLength As Byte
        Private __Probability As Double

        Private __ChildLeft As zlibVBNET.Huffman.Node
        Private __ChildRight As zlibVBNET.Huffman.Node

        '*
        '* Is this node is part of a collection
        '*
        Private __ParentCollection As zlibVBNET.Huffman.Table.NodeCollection ' <-- Collection Containing the Node
        Private __Index As Integer                                      ' <-- Index of this node inside the collection

        ''' <summary>
        ''' (Internal) (Overloaded)
        ''' Initializes a new instance of the Node class.
        ''' </summary>
        Friend Sub New()
            __Value = Nothing
            __Code = 0
            __CodeLength = 0
            __Probability = 0
            __ChildLeft = Nothing
            __ChildRight = Nothing
        End Sub

        ''' <summary>
        ''' (Internal) (Overloaded)
        ''' Initializes a new instance of the Node class.
        ''' </summary>
        Friend Sub New(ByVal Value As Object)
            __Value = Value
            __Probability = 0
            __Code = 0
            __CodeLength = 0
            __ChildLeft = Nothing
            __ChildRight = Nothing
        End Sub

        ''' <summary>
        ''' (Internal) (Overloaded)
        ''' Initializes a new instance of the Node class.
        ''' </summary>
        Friend Sub New(ByVal Value As Object, ByVal CodeLength As Byte)
            __Value = Value
            __Probability = 0
            __Code = 0
            __CodeLength = CodeLength
            __ChildLeft = Nothing
            __ChildRight = Nothing
        End Sub

        ''' <summary>
        ''' (Internal) (Overloaded)
        ''' Initializes a new instance of the Node class.
        ''' </summary>
        Friend Sub New(ByVal Value As Object, ByVal CodeLength As Byte, ByVal Code As Long)
            __Value = Value
            __Probability = 0
            __Code = Code
            __CodeLength = CodeLength
            __ChildLeft = Nothing
            __ChildRight = Nothing
        End Sub

        ''' <summary>
        ''' The Value of the Node.
        ''' </summary>
        ''' <returns>An object with the value of the node, if the node is a leaf, or else a null reference (Nothing in Visual Basic).</returns>
        Public ReadOnly Property Value() As Object
            Get
                If (Not __ChildLeft Is Nothing) OrElse (__ChildRight Is Nothing) Then
                    Return Nothing
                Else
                    Return __Value
                End If
            End Get
        End Property

        ''' <summary>
        ''' The probability of the code appears in an ensembled message.
        ''' 
        ''' If the node has sub-trees below it, the probability of the node is
        ''' the sum of the probabilities of its sub-trees.
        ''' </summary>
        ''' <returns>A number between 0 and 1 that represents the probability of the code.</returns>
        Public ReadOnly Property Probability() As Double
            Get
                Return __Probability
            End Get
        End Property

        ''' <summary>
        ''' The Huffman Code of the node.
        ''' 
        ''' Must be used in association with CodeLength.
        ''' </summary>
        ''' <returns>The Huffman Code of the node.</returns>
        Public ReadOnly Property Code() As Long
            Get
                '*
                '* Just to make sure we have it right
                '*
                Return (__Code And Masks.BitMask(__CodeLength))
            End Get
        End Property

        ''' <summary>
        ''' The binary string representation of the Huffman Code.
        ''' 
        ''' The character "0" means a bit 0 and the character "1" a bit 1.
        ''' The most significant bit (MSB) is represented on the left.
        ''' For instance the string "011001" represents the code 25 
        ''' with a CodeLength of 6 bits.
        ''' </summary>
        ''' <returns>A string with length equals to the CodeLength.</returns>
        Public ReadOnly Property BinaryCode() As String
            Get
                Dim strExit As String
                Dim lngAux As Long

                lngAux = __Code
                Do
                    strExit = Masks.HexMask(CByte(lngAux And Masks.BitMask(4))) & strExit
                    lngAux >>= 4
                Loop While (lngAux <> 0)

                Return (Right(strExit.PadLeft(__CodeLength, "0"c), __CodeLength))
            End Get
        End Property

        ''' <summary>
        ''' The length (in bits) of the Huffman Code.
        ''' </summary>
        ''' <returns>A number indicating the length of the Huffman Code.</returns>
        Public ReadOnly Property CodeLength() As Byte
            Get
                Return __CodeLength
            End Get
        End Property

        ''' <summary>
        ''' The sub-tree at the left part of the node.
        ''' </summary>
        ''' <returns>A reference for the left sub-tree of the node.</returns>
        Public ReadOnly Property ChildLeft() As zlibVBNET.Huffman.Node
            Get
                Return __ChildLeft
            End Get
        End Property

        ''' <summary>
        ''' The sub-tree at the right part of the node.
        ''' </summary>
        ''' <returns>A reference for the right sub-tree of the node.</returns>
        Public ReadOnly Property ChildRight() As zlibVBNET.Huffman.Node
            Get
                Return __ChildRight
            End Get
        End Property

        ''' <summary>
        ''' (Internal)
        ''' The index of the node when inside a Node Collection.
        ''' </summary>
        ''' <returns>A number indicating the index (zero based) of the node inside a Node Collection.</returns>
        Friend Property Index() As Integer
            Get
                Return __Index
            End Get
            Set(ByVal Value As Integer)
                __Index = Value
            End Set
        End Property

        ''' <summary>
        ''' (Internal)
        ''' If the node is part of a Node Collection, return a reference for the collection.
        ''' </summary>
        ''' <returns>The Node Collection where the node resides.</returns>
        Friend Property ParentCollection() As zlibVBNET.Huffman.Table.NodeCollection
            Get
                Return __ParentCollection
            End Get
            Set(ByVal Value As zlibVBNET.Huffman.Table.NodeCollection)
                __ParentCollection = Value
            End Set
        End Property

        ''' <summary>
        ''' (Internal)
        ''' Sets the value of the node.
        ''' </summary>
        ''' <param name="Value">The new value for the node.</param>
        Friend Sub SetValue(ByVal Value As Object)
            __Value = Value
        End Sub

        ''' <summary>
        ''' (Internal)
        ''' Sets the probability of the node.
        ''' </summary>
        ''' <param name="Value">The new probability of the node.</param>
        Friend Sub SetProbability(ByVal Value As Double)
            __Probability = Value
        End Sub

        ''' <summary>
        ''' (Internal)
        ''' Sets the code length of the node.
        ''' 
        ''' If the node has sub-trees also corrects the code length for each sub-tree.
        ''' </summary>
        ''' <param name="CodeLength">The new code length of the node.</param>
        Friend Sub SetCodeLength(ByVal CodeLength As Byte)

            If (__CodeLength = CodeLength) Then
                Exit Sub
            End If

            __CodeLength = CodeLength

            '*
            '* Adjust the code length for each child node
            '*
            If (Not __ChildLeft Is Nothing) Then
                __ChildLeft.SetCodeLength(CodeLength + 1)
            End If

            If (Not __ChildRight Is Nothing) Then
                __ChildRight.SetCodeLength(CodeLength + 1)
            End If
        End Sub

        ''' <summary>
        ''' (Internal)
        ''' Sets the Huffman Code of the node.
        ''' 
        ''' If the node has sub-trees also corrects the Huffman Code for each sub-tree.
        ''' To the left sub-tree is assigned the code 0, and the code 1 is assigned to the
        ''' right sub-tree.
        ''' </summary>
        ''' <param name="Code">The new Huffman Code of the node.</param>
        Friend Sub SetCode(ByVal Code As Long)
            __Code = Code

            '*
            '* Adjust the code for each child node
            '*
            If (Not __ChildLeft Is Nothing) Then
                __ChildLeft.SetCode(__Code << 1)
            End If

            If (Not __ChildRight Is Nothing) Then
                __ChildRight.SetCode((__Code << 1) + 1)
            End If
        End Sub

        ''' <summary>
        ''' (Internal)
        ''' Sets the left sub-tree for the node.
        ''' </summary>
        ''' <param name="Child">The root of the left sub-tree.</param>
        Friend Sub SetChildLeft(ByVal Child As zlibVBNET.Huffman.Node)
            __ChildLeft = Child
            If (Not Child Is Nothing) AndAlso (Not __ChildRight Is Nothing) Then
                __Probability = __ChildLeft.Probability + __ChildRight.Probability
            End If
        End Sub

        ''' <summary>
        ''' (Internal)
        ''' Sets the right sub-tree for the node.
        ''' </summary>
        ''' <param name="Child">The root of the right sub-tree.</param>
        Friend Sub SetChildRight(ByVal Child As zlibVBNET.Huffman.Node)
            __ChildRight = Child
            If (Not Child Is Nothing) AndAlso (Not __ChildLeft Is Nothing) Then
                __Probability = __ChildLeft.Probability + __ChildRight.Probability
            End If
        End Sub

    End Class

    ''' <summary>
    ''' The Table Class is the constructor engine for a Huffman Code.
    ''' </summary>
    Public Class Table

        ''' <summary>
        ''' Represents the Huffman Code table, that's used to encode or decode messages
        ''' encoded with a Huffman Tree structure.
        ''' </summary>
        Public Class CodeValueCollection
            Inherits System.Collections.Specialized.NameObjectCollectionBase

            ''' <summary>
            ''' (Internal)
            ''' Initializes a new instance of the Code Value Collection class.
            ''' </summary>
            Friend Sub New()
                MyBase.New()
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Adds a new Code-Value pair.
            ''' </summary>
            ''' <param name="Name">The Huffman Code to be added, in its binary string representation.</param>
            ''' <param name="Value">The value that the code represents.</param>
            Friend Sub Add(ByVal Name As String, ByVal Value As Object)
                MyBase.BaseSet(Name, Value)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Remove a Code-Value pair.
            ''' </summary>
            ''' <param name="Name">The Huffman Code to be removed, in its binary string representation.</param>
            Friend Overloads Sub Remove(ByVal Name As String)
                MyBase.BaseRemove(Name)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Remove a Code-Value pair.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the table of the Huffman Code to be removed.</param>
            Friend Overloads Sub Remove(ByVal Index As Integer)
                MyBase.BaseRemoveAt(Index)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Sets the Value of a Huffman Code.
            ''' </summary>
            ''' <param name="Name">The Huffman Code to be setted, in its binary string representation.</param>
            ''' <param name="Value">The Value that the Huffman Code represents.</param>
            Friend Overloads Sub [Set](ByVal Name As String, ByVal Value As Object)
                MyBase.BaseSet(Name, Value)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Sets the Value of a Huffman Code.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the table of the Huffman Code to be changed.</param>
            ''' <param name="Value"></param>
            Friend Overloads Sub [Set](ByVal Index As Integer, ByVal Value As Object)
                MyBase.BaseSet(Index, Value)
            End Sub

            ''' <summary>
            ''' The Value represented by a Huffman Code.
            ''' </summary>
            ''' <param name="Name">The Huffman Code in its binary string representation.</param>
            ''' <returns>An object with the value that the Huffman Code represents.</returns>
            Public Overloads Property Item(ByVal Name As String) As Object
                Get
                    Return MyBase.BaseGet(Name)
                End Get
                Set(ByVal Value As Object)
                    Call MyBase.BaseSet(Name, Value)
                End Set
            End Property

            ''' <summary>
            ''' The Value represented by a Huffman Code.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the table of the Huffman Code.</param>
            ''' <returns>An object with the value that the Huffman Code represents.</returns>
            Public Overloads Property Item(ByVal Index As Integer) As Object
                Get
                    Return MyBase.BaseGet(Index)
                End Get
                Set(ByVal Value As Object)
                    Call MyBase.BaseSet(Index, Value)
                End Set
            End Property

            ''' <summary>
            ''' All the Huffman Codes stored in the collection.
            ''' </summary>
            ''' <returns>A string array containing all the binary string 
            ''' representation of the Huffman Codes sorted by CodeLength.</returns>
            Public ReadOnly Property AllKeys() As String()
                Get
                    Static aKeys() As String = {}

                    If (aKeys.Length <> Me.Count) Then
                        '*
                        '* Transfer the Keys to an array
                        '*
                        aKeys = New String() {}
                        For Each Key As String In MyBase.Keys
                            ReDim Preserve aKeys(aKeys.Length)
                            aKeys(aKeys.Length - 1) = Key
                        Next

                        '*
                        '* Sort the keys
                        '*
                        Call SortKeys(aKeys)
                    End If

                    Return (aKeys)
                End Get
            End Property

            ''' <summary>
            ''' (Private)
            ''' Sorts the Huffman Codes by Code Length.
            ''' </summary>
            ''' <param name="aKeys">A string array with all the Huffman Codes of the collection.</param>
            Private Sub SortKeys(ByRef aKeys As String())

                '*
                '* First we sort by Length
                '*
                Call DoSortLength(aKeys, 0, aKeys.Length - 1)
                Call DoSortLength(aKeys, 0, aKeys.Length - 1)

                '*
                '* Then we sort each length
                '*
                Dim intLeft As Integer
                Dim intRight As Integer
                Dim intKeyLength As Integer
                For Each Key As String In aKeys
                    If (intKeyLength < Key.Length) Then
                        If (intRight - intLeft > 1) Then
                            Call DoSortKeys(aKeys, intLeft, intRight - 1)
                            Call DoSortKeys(aKeys, intLeft, intRight - 1)
                        End If

                        intKeyLength = Key.Length
                        intLeft = intRight
                    End If
                    intRight += 1
                Next
                '*
                '* Chekc if there is something to do
                '*
                If (intLeft <> intRight) Then
                    Call DoSortKeys(aKeys, intLeft, intRight - 1)
                    Call DoSortKeys(aKeys, intLeft, intRight - 1)
                End If
            End Sub

            ''' <summary>
            ''' (Private)
            ''' Implements the Quick Sort algorithm to sort the array of Huffman Codes.
            ''' </summary>
            ''' <param name="aKeys">A string array with all the Huffman Codes of the collection.</param>
            ''' <param name="LeftIndex">The Left Index of Quick Sort algorithm.</param>
            ''' <param name="RightIndex">The Right Index of Quick Sort algorithm.</param>
            Private Sub DoSortKeys(ByRef aKeys As String(), ByVal LeftIndex As Integer, ByVal RightIndex As Integer)

                Dim intPivot As Integer
                Dim intLeftAux As Integer
                Dim intRightAux As Integer

                intLeftAux = LeftIndex
                intRightAux = RightIndex

                '*
                '* Elects a Pivot
                '*
                intPivot = (RightIndex - LeftIndex) \ 2 + LeftIndex

                Do While (intLeftAux <= intRightAux)
                    '*
                    '* Check if the item pointed by intLeftAux are 
                    '* smaller than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the right.
                    '*
                    Do While (aKeys(intLeftAux) < aKeys(intPivot))
                        intLeftAux += 1
                    Loop

                    '*
                    '* Check if the item pointed by intRightAux are 
                    '* greater than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the left.
                    '*
                    Do While (aKeys(intRightAux) > aKeys(intPivot))
                        intRightAux -= 1
                    Loop

                    '*
                    '* We swap the nodes if they are on the wrong side
                    '* 
                    If (intLeftAux <= intRightAux) Then
                        Dim strAux As String
                        strAux = aKeys(intLeftAux)
                        aKeys(intLeftAux) = aKeys(intRightAux)
                        aKeys(intRightAux) = strAux

                        '*
                        '* Move the pointers on each directions
                        '*
                        intLeftAux += 1
                        intRightAux -= 1
                    End If
                Loop

                '*
                '* Just a note about the algorithm:
                '* 
                '* At this point all the items of the array at 
                '* the right of the pivot are smaller (in this particular case)
                '* than the pivot. And the items on the left are greater.
                '*

                '*
                '* If the Right Pointer are beyond the Initial Left Index
                '* Call itself recursively doing the LeftIndex..intRightAux
                '* part of the array
                '*
                If (LeftIndex < intRightAux) Then
                    Call DoSortKeys(aKeys, LeftIndex, intRightAux)
                End If

                '*
                '* If the Left Pointer are beyond the Initial Right Index
                '* Call itself recursively doing the intLeftAux..RightIndex
                '* part of the array
                '*
                If (RightIndex > intLeftAux) Then
                    Call DoSortKeys(aKeys, intLeftAux, RightIndex)
                End If

            End Sub

            ''' <summary>
            ''' (Private)
            ''' Implements the Quick Sort algorithm to sort the array of Huffman Codes by the Code Lengths.
            ''' </summary>
            ''' <param name="aKeys">A string array with all the Huffman Codes of the collection.</param>
            ''' <param name="LeftIndex">The Left Index of Quick Sort algorithm.</param>
            ''' <param name="RightIndex">The Right Index of Quick Sort algorithm.</param>
            Private Sub DoSortLength(ByRef aKeys As String(), ByVal LeftIndex As Integer, ByVal RightIndex As Integer)

                Dim intPivot As Integer
                Dim intLeftAux As Integer
                Dim intRightAux As Integer

                intLeftAux = LeftIndex
                intRightAux = RightIndex

                '*
                '* Elects a Pivot
                '*
                intPivot = (RightIndex - LeftIndex) \ 2 + LeftIndex

                Do While (intLeftAux <= intRightAux)
                    '*
                    '* Check if the item pointed by intLeftAux are 
                    '* smaller than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the right.
                    '*
                    Do While (aKeys(intLeftAux).Length < aKeys(intPivot).Length)
                        intLeftAux += 1
                    Loop

                    '*
                    '* Check if the item pointed by intRightAux are 
                    '* greater than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the left.
                    '*
                    Do While (aKeys(intRightAux).Length > aKeys(intPivot).Length)
                        intRightAux -= 1
                    Loop

                    '*
                    '* We swap the nodes if they are on the wrong side
                    '* 
                    If (intLeftAux <= intRightAux) Then
                        Dim strAux As String
                        strAux = aKeys(intLeftAux)
                        aKeys(intLeftAux) = aKeys(intRightAux)
                        aKeys(intRightAux) = strAux

                        '*
                        '* Move the pointers on each directions
                        '*
                        intLeftAux += 1
                        intRightAux -= 1
                    End If
                Loop

                '*
                '* Just a note about the algorithm:
                '* 
                '* At this point all the items of the array at 
                '* the right of the pivot are smaller (in this particular case)
                '* than the pivot. And the items on the left are greater.
                '*

                '*
                '* If the Right Pointer are beyond the Initial Left Index
                '* Call itself recursively doing the LeftIndex..intRightAux
                '* part of the array
                '*
                If (LeftIndex < intRightAux) Then
                    Call DoSortLength(aKeys, LeftIndex, intRightAux)
                End If

                '*
                '* If the Left Pointer are beyond the Initial Right Index
                '* Call itself recursively doing the intLeftAux..RightIndex
                '* part of the array
                '*
                If (RightIndex > intLeftAux) Then
                    Call DoSortLength(aKeys, intLeftAux, RightIndex)
                End If

            End Sub

        End Class

        ''' <summary>
        ''' A Collection of all the codes of the Huffman Table by Code Lengths.
        ''' </summary>
        Public Class CodeLengthCollection
            Inherits System.Collections.Specialized.NameObjectCollectionBase

            ''' <summary>
            ''' (Internal)
            ''' Initializes a new instance of the Code Value Collection class.
            ''' </summary>
            Friend Sub New()
                MyBase.New()
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Adds a new Code Length into the collection.
            ''' </summary>
            ''' <param name="Name">The string representation (leading zeros) of the code length.</param>
            ''' <param name="Value">A string array with all the codes of the code length represented by Name.</param>
            Friend Sub Add(ByVal Name As String, ByVal Value As String())
                MyBase.BaseSet(Name, Value)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Removes a Code Length from the collection.
            ''' </summary>
            ''' <param name="Name">The string representation (leading zeros) of the code length.</param>
            Friend Overloads Sub Remove(ByVal Name As String)
                MyBase.BaseRemove(Name)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Removes a Code Length from the collection.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the Code Length to be removed.</param>
            Friend Overloads Sub Remove(ByVal Index As Integer)
                MyBase.BaseRemoveAt(Index)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Sets the string array containing all the codes of the code length represented by Name.
            ''' </summary>
            ''' <param name="Name">The string representation (leading zeros) of the code length.</param>
            ''' <param name="Value">A string array with all the codes of the code length represented by Name.</param>
            Friend Overloads Sub [Set](ByVal Name As String, ByVal Value As String())
                MyBase.BaseSet(Name, Value)
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Sets the string array containing all the codes of the code length represented by Name.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the Code Length.</param>
            ''' <param name="Value">A string array with all the codes of the code length represented by Name.</param>
            Friend Overloads Sub [Set](ByVal Index As Integer, ByVal Value As String())
                MyBase.BaseSet(Index, Value)
            End Sub

            ''' <summary>
            ''' All the Huffman Codes with the same Code Length.
            ''' </summary>
            ''' <param name="Name">The string representation (leading zeros) of the code length.</param>
            ''' <returns>A string array with all the Huffman Codes with the same Code Length.</returns>
            Public Property Item(ByVal Name As String) As String()
                Get
                    Return MyBase.BaseGet(Name)
                End Get
                Set(ByVal Value As String())
                    Call MyBase.BaseSet(Name, Value)
                End Set
            End Property

            ''' <summary>
            ''' All the Huffman Codes with the same Code Length.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the Code Length.</param>
            ''' <returns>A string array with all the Huffman Codes with the same Code Length.</returns>
            Public Property Item(ByVal Index As Integer) As String()
                Get
                    Return MyBase.BaseGet(Index)
                End Get
                Set(ByVal Value As String())
                    Call MyBase.BaseSet(Index, Value)
                End Set
            End Property

            ''' <summary>
            ''' All the Code Lengths stored in the collection.
            ''' </summary>
            ''' <returns>A sorted string array containing all the Code Lengths.</returns>
            Public ReadOnly Property AllKeys() As String()
                Get
                    Static aKeys() As String = {}

                    If (aKeys.Length <> Me.Count) Then
                        '*
                        '* Transfer the Keys to an array
                        '*
                        aKeys = New String() {}
                        For Each Key As String In MyBase.Keys
                            ReDim Preserve aKeys(aKeys.Length)
                            aKeys(aKeys.Length - 1) = Key
                        Next

                        '*
                        '* Sort the keys
                        '*
                        Call Array.Sort(aKeys)
                    End If

                    Return (aKeys)
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Represents a collection of Huffman tree nodes, before the complete tree takes shape.
        ''' </summary>
        Public Class NodeCollection
            Inherits System.Collections.CollectionBase

            ''' <summary>
            ''' (Internal)
            ''' Initializes a new instance of a Node Collection.
            ''' </summary>
            Friend Sub New()
                MyBase.New()
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Adds a new node to the collection.
            ''' </summary>
            ''' <param name="Value">The node to be added.</param>
            Friend Sub Add(ByVal Value As zlibVBNET.Huffman.Node)
                MyBase.List.Add(Value)
                Value.Index = Count - 1
                Value.ParentCollection = Me
            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Removes a node from the collection.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the node to be removed.</param>
            Friend Sub Remove(ByVal Index As Integer)
                Item(Index).ParentCollection = Nothing ' <-- remove info about the parent collection
                Item(Index).Index = Nothing            ' <-- corrects the index inside the node 
                MyBase.RemoveAt(Index)                 ' <-- remove the node from the collection 
            End Sub

            ''' <summary>
            ''' A node of the collection.
            ''' </summary>
            ''' <param name="Index">The Index (zero based) of the node.</param>
            ''' <returns>A node of the collection.</returns>
            Public Property Item(ByVal Index As Integer) As zlibVBNET.Huffman.Node
                Get
                    Return MyBase.List(Index)
                End Get
                Set(ByVal Value As zlibVBNET.Huffman.Node)
                    MyBase.List.Item(Index) = Value
                End Set
            End Property

            ''' <summary>
            ''' (Internal)
            ''' Sorts descendently all the nodes by its probabilities.
            ''' </summary>
            Friend Sub SortNodes()

                '*
                '* The Quick Sort algorithm demands that
                '* we process the array twice in order to
                '* coorectly sort the array.
                '*
                Call DoQuickSort(0, Me.Count - 1)
                Call DoQuickSort(0, Me.Count - 1)

            End Sub

            ''' <summary>
            ''' (Private)
            ''' Performs a Quick Sort algorithm in all nodes to sort them by its probabilities.
            ''' </summary>
            ''' <param name="LeftIndex">The Left Index of Quick Sort algorithm.</param>
            ''' <param name="RightIndex">The Right Index of Quick Sort algorithm.</param>
            Private Sub DoQuickSort(ByVal LeftIndex As Integer, ByVal RightIndex As Integer)

                Dim intPivot As Integer
                Dim intLeftAux As Integer
                Dim intRightAux As Integer

                intLeftAux = LeftIndex
                intRightAux = RightIndex

                '*
                '* Elects a Pivot
                '*
                intPivot = (RightIndex - LeftIndex) \ 2 + LeftIndex

                Do While (intLeftAux <= intRightAux)
                    '*
                    '* Because we have a descending order on the nodes
                    '* we inverted the comparations.
                    '*

                    '*
                    '* Check if the item pointed by intLeftAux are 
                    '* greater than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the right.
                    '*
                    Do While (Item(intLeftAux).Probability > Item(intPivot).Probability)
                        intLeftAux += 1
                    Loop

                    '*
                    '* Check if the item pointed by intRightAux are 
                    '* smaller than the pivot.
                    '*
                    '* It means that the Item is already sorted, 
                    '* then we move one position to the left.
                    '*
                    Do While (Item(intRightAux).Probability < Item(intPivot).Probability)
                        intRightAux -= 1
                    Loop

                    '*
                    '* We swap the nodes if they are on the wrong side
                    '* 
                    If (intLeftAux <= intRightAux) Then
                        SwapNodes(intLeftAux, intRightAux)

                        '*
                        '* Move the pointers on each directions
                        '*
                        intLeftAux += 1
                        intRightAux -= 1
                    End If
                Loop

                '*
                '* Just a note about the algorithm:
                '* 
                '* At this point all the items of the array at 
                '* the right of the pivot are smaller (in this particular case)
                '* than the pivot. And the items on the left are greater.
                '*

                '*
                '* If the Right Pointer are beyond the Initial Left Index
                '* Call itself recursively doing the LeftIndex..intRightAux
                '* part of the array
                '*
                If (LeftIndex < intRightAux) Then
                    Call DoQuickSort(LeftIndex, intRightAux)
                End If

                '*
                '* If the Left Pointer are beyond the Initial Right Index
                '* Call itself recursively doing the intLeftAux..RightIndex
                '* part of the array
                '*
                If (RightIndex > intLeftAux) Then
                    Call DoQuickSort(intLeftAux, RightIndex)
                End If

            End Sub

            ''' <summary>
            ''' (Internal)
            ''' Sorts a node by its probabilities.
            ''' 
            ''' Once the node list is sorted it's easier, when adding a node to the list, 
            ''' to just sort this newly added item.
            ''' </summary>
            ''' <param name="Index"></param>
            Friend Sub SortNodeUp(ByVal Index As Integer)

                Dim intAux As Integer
                Dim oNode As zlibVBNET.Huffman.Node

                intAux = Index - 1
                If (intAux < 0) Then
                    '*
                    '* Already at the top of the list
                    '*
                    Exit Sub
                End If

                '*
                '* Search for the right position
                '*
                Do While ((intAux > 0) AndAlso Item(intAux).Probability < Item(Index).Probability)
                    intAux -= 1
                Loop

                oNode = Item(Index)
                Call Remove(Index)                      ' <-- Remove the node
                Call MyBase.List.Insert(intAux, oNode)  ' <-- Insert at the right position
                oNode.Index = intAux                    ' <-- Corrects the index inside the node

            End Sub

            ''' <summary>
            ''' (Private)
            ''' Swaps two nodes in the list.
            ''' </summary>
            ''' <param name="Index1">An Index (zero based) of a node to be swapped.</param>
            ''' <param name="Index2">An Index (zero based) of a node to be swapped.</param>
            Private Sub SwapNodes(ByVal Index1 As Integer, ByVal Index2 As Integer)

                Dim newNode As zlibVBNET.Huffman.Node
                Dim oldNode As zlibVBNET.Huffman.Node

                '*
                '* Retrieves the two nodes to be swaped
                '*
                oldNode = Item(Index1)
                newNode = Item(Index2)

                '*
                '* Swaps the nodes
                '*
                oldNode.Index = Index2
                newNode.Index = Index1
                MyBase.List.Item(Index2) = oldNode
                MyBase.List.Item(Index1) = newNode

            End Sub

        End Class

        ''' <summary>
        ''' Represents a custom alphabet for the Huffman Coding Algorithm.
        ''' 
        ''' By default this implementation uses all the 256 possible values
        ''' of one byte as its alphabet.
        ''' 
        ''' Due the nature of the Huffman Coding Algorithm one need to know
        ''' before hand all the possible message codes in order to compute
        ''' its probabilities on any given message.
        ''' 
        ''' This implementation of the Huffman Coding Algorithm allows the
        ''' definition of Custom Alphabets for the process of a message.
        ''' 
        ''' A Custom Alphabet is an array of byte arrays. Each byte array is 
        ''' a Message Code. This implementation does not allow Message Codes 
        ''' of different lengths.
        ''' </summary>
        Public Class Alphabet
            Inherits System.Collections.ArrayList

            ''' <summary>
            ''' (Internal)
            ''' Occurs immediately before a Message Code is added in the Alphabet.
            ''' </summary>
            ''' <param name="CanAdd">If true tell the Alphabet class that it can add the MessageCode.</param>
            Friend Event AddingMessageCode(ByRef CanAdd As Boolean)

            ''' <summary>
            ''' (Internal)
            ''' Occurs immediately after a Message Code is added in the Alphabet.
            ''' </summary>
            Friend Event MessageCodeAdded()

            ''' <summary>
            ''' (Internal)
            ''' Occurs immediately before a Message Code is removed in the Alphabet.
            ''' </summary>
            ''' <param name="CanRemove">If true tell the Alphabet class that it can remove the MessageCode.</param>
            Friend Event RemovingMessageCode(ByRef CanRemove As Boolean)

            ''' <summary>
            ''' (Internal)
            ''' Occurs immediately after a Message Code is removed in the Alphabet.
            ''' </summary>
            Friend Event MessageCodeRemoved()

            ''' <summary>
            ''' Initialize a new instance of the Alphabet Class.
            ''' </summary>
            Public Sub New()
                MyBase.New()
            End Sub

            ''' <summary>
            ''' Initialize a new instance of the Alphabet Class.
            ''' </summary>
            ''' <param name="Capacity">The number of elements that the new Alphabet is initially capable of storing.</param>
            Public Sub New(ByVal Capacity As Integer)
                MyBase.New(Capacity)
            End Sub

            ''' <summary>
            ''' Adds a new Message Code to the Alphabet.
            ''' 
            ''' Returns the Index (zero based) of the newly added Message Code,
            ''' or -1 if the Message Code could not be added.
            ''' </summary>
            ''' <param name="MessageCode">A byte array that represents the Message Code to be added.</param>
            ''' <returns>The Index (zero based) of the newly added Message Code.</returns>
            Public Overrides Function Add(ByVal MessageCode As Object) As Integer
                Try
                    CheckMessageCode(MessageCode)
                Catch
                    Throw
                End Try

                If (MyBase.Count > 0) Then
                    If (MyBase.Item(0).Length <> MessageCode.Length) Then
                        Throw New ArgumentException("This implementation does not support Message Codes of different lengths.", "MessageCode")
                    End If
                End If

                Dim flgCanAdd As Boolean
                RaiseEvent AddingMessageCode(flgCanAdd)
                If (flgCanAdd) Then
                    Return (MyBase.Add(MessageCode))
                    RaiseEvent MessageCodeAdded()
                Else
                    Return (-1)
                End If
            End Function

            ''' <summary>
            ''' (Overloaded)
            ''' Remove a Message Code from the alphabet.
            ''' </summary>
            ''' <param name="MessageCode">The Message Code to be removed.</param>
            Public Overloads Overrides Sub Remove(ByVal MessageCode As Object)
                Try
                    CheckMessageCode(MessageCode)
                Catch
                    Throw
                End Try

                Dim flgCanRemove As Boolean
                RaiseEvent RemovingMessageCode(flgCanRemove)
                If (flgCanRemove) Then
                    MyBase.Remove(MessageCode)
                    RaiseEvent MessageCodeRemoved()
                End If
            End Sub

            ''' <summary>
            ''' (Overloaded)
            ''' Remove a Message Code from the alphabet.
            ''' </summary>
            ''' <param name="Index">The Index of the Message Code to be removed.</param>
            Public Overloads Sub Remove(ByVal Index As Integer)
                MyBase.RemoveAt(Index)
            End Sub

            ''' <summary>
            ''' (Private)
            ''' Check if a Message Code is a byte array.
            ''' 
            ''' If the Message Code fails in any check an exception is thrown.
            ''' 
            ''' Exceptions that can be thrown:
            ''' 
            ''' ArgumentNullException - MessageCode is Nothing
            ''' ArgumentException     - MessageCode is not an Array
            '''                         MessageCode is empty
            '''                         MessageCode is not a Byte Array
            '''                         MessageCode is already in the alphabet
            ''' </summary>
            ''' <param name="MessageCode">The Message Code to be checked.</param>
            Private Sub CheckMessageCode(ByVal MessageCode As Object)

                '*
                '* Check if MessageCode is Nothing
                '*
                If (MessageCode Is Nothing) Then
                    Throw New ArgumentNullException("MessageCode", "MessageCode cannot be null (Nothing in Visual Basic).")
                End If

                '*
                '* Check if MessageCode is an Array
                '*
                If (Not MessageCode.GetType.IsArray) Then
                    Throw New ArgumentException("MessageCode must be an array of bytes.", "MessageCode")
                End If

                '*
                '* Is the array empty?
                '*
                If (MessageCode.Length = 0) Then
                    Throw New ArgumentException("MessageCode cannot be empty.", "MessageCode")
                End If

                '*
                '* Ok it's an array, and it's not empty, but it's a byte array
                '*
                If (Not MessageCode(0).GetType.Equals(GetType(Byte))) Then
                    Throw New ArgumentException("MessageCode must be an array of bytes", "MessageCode")
                End If

                '*
                '* Check if the MessageCode isn't alread in the list
                '*
                If (MyBase.Contains(MessageCode) <> (-1)) Then
                    Throw New ArgumentException("MessageCode already in the alphabet.", "MessageCode")
                End If

            End Sub

        End Class

        Private __DataStream As System.IO.Stream        ' <-- Data Stream where the messages will be read from

        Private __Table As zlibVBNET.Huffman.Node            ' <-- The Table as it is constructed

        Private __Chunk As Integer                      ' <-- The current data Chunk
        Private __ChunkLength As Long                   ' <-- Chunk Length (Must be multiple of MessageCodeLength)
        Private __CodeTable() As Object                 ' <-- Bi-dimensional Table of codes

        Private __Codes As CodeLengthCollection         ' <-- Collection with all codes by code length index
        Private __Values As CodeValueCollection         ' <-- Collection with all the values and its codes

        Private WithEvents __Alphabet As Alphabet       ' <-- A custom alphabet

        ''' <summary>
        ''' (Overloaded)
        ''' Initializes a new instance of a Table Class.
        ''' </summary>
        Public Sub New()
            __DataStream = Nothing
            __Alphabet = Nothing
            __Chunk = 0
            __ChunkLength = (-1)
            __Table = New zlibVBNET.Huffman.Node
        End Sub

        ''' <summary>
        ''' (Overloaded)
        ''' Initializes a new instance of a Table Class.
        ''' </summary>
        ''' <param name="DataStream">A stream of data to be analized and generate Huffman Codes for all Message Codes (default bytes).</param>
        Public Sub New(ByVal DataStream As System.IO.Stream)
            __DataStream = DataStream
            __Alphabet = Nothing
            __Chunk = 0
            __ChunkLength = (-1)
            __Table = New zlibVBNET.Huffman.Node
        End Sub

        ''' <summary>
        ''' (Overloaded)
        ''' Initializes a new instance of a Table Class.
        ''' </summary>
        ''' <param name="DataStream">A stream of data to be analized and generate Huffman Codes for all Message Codes (default bytes).</param>
        ''' <param name="CustomAlphabet">An Alphabet that will be used to generate the Huffman Tree.</param>
        Public Sub New(ByVal DataStream As System.IO.Stream, ByVal CustomAlphabet As Alphabet)
            __DataStream = DataStream
            __Alphabet = CustomAlphabet
            __Chunk = 0
            __ChunkLength = (-1)
            __Table = New zlibVBNET.Huffman.Node
        End Sub

        ''' <summary>
        ''' The number of chunks already processed.
        ''' </summary>
        ''' <returns>A number that indicates the number of chunks already processed.</returns>
        Public ReadOnly Property Chunk() As Integer
            Get
                Return __Chunk
            End Get
        End Property

        ''' <summary>
        ''' The length of each chunk.
        ''' </summary>
        ''' <returns>A number that indicates the length of each chunk.</returns>
        Public Property ChunkLength() As Long
            Get
                Return __ChunkLength
            End Get
            Set(ByVal Value As Long)
                If (Value < 1) AndAlso (Value <> (-1)) Then
                    Throw New ArgumentException("ChunkLength must be a positive number, or -1 for an unknown chunk length. The value (" & Value & ") is invalid.", "Value")
                End If
                __ChunkLength = Value
            End Set
        End Property

        ''' <summary>
        ''' Process and returns the Huffman Tree.
        ''' </summary>
        ''' <returns>The Huffman Tree structure.</returns>
        Public ReadOnly Property HuffmanTree() As zlibVBNET.Huffman.Node
            Get
                If ((Not __DataStream Is Nothing) AndAlso ((__Table.ChildLeft Is Nothing) OrElse (__Table.ChildRight Is Nothing))) Then
                    Call BuildTable()
                End If

                Return __Table
            End Get
        End Property

        ''' <summary>
        ''' A dictionary like collection with all the Huffman Codes generated, by code length.
        ''' 
        ''' See CodeLengthCollection.
        ''' </summary>
        ''' <returns>A CodeLengthCollection with all the Huffman Codes generated.</returns>
        Public ReadOnly Property CodeTable() As CodeLengthCollection
            Get
                Return __Codes
            End Get
        End Property

        ''' <summary>
        ''' A dictionary like collection with all the Huffman Codes generated.
        ''' 
        ''' See CodeValueCollection.
        ''' </summary>
        ''' <returns>A CodeValueCollection with all the Huffman Codes generated.</returns>
        Public ReadOnly Property ValueTable() As CodeValueCollection
            Get
                Return __Values
            End Get
        End Property

        ''' <summary>
        ''' Reads or sets the Custom Alphabet for the Huffman Codes.
        ''' </summary>
        ''' <returns></returns>
        Public Property CustomAlphabet() As Alphabet
            Get
                Return __Alphabet
            End Get
            Set(ByVal Value As Alphabet)
                If (Not __DataStream Is Nothing) Then
                    If (__Chunk > 0) Then
                        Throw New zlibVBNET.Exceptions.InvalidAlphabetChange("The alphabet cannot be changed after processing.")
                    End If
                End If
                __Alphabet = Value
                __Table = New zlibVBNET.Huffman.Node
            End Set
        End Property

        ''' <summary>
        ''' (Private)
        ''' Treat the event of AddingMessageCode to check if a Message Code can be added to the alphabet.
        ''' </summary>
        ''' <param name="CanAdd">Tells the alphabet if it can add the Message Code.</param>
        Private Sub AddingMessageCodeAlphabet(ByRef CanAdd As Boolean) Handles __Alphabet.AddingMessageCode
            CanAdd = (__DataStream Is Nothing) OrElse (__Chunk = 0)
        End Sub

        ''' <summary>
        ''' (Private)
        ''' Treat the event of AddingMessageCode to check if a Message Code can be removed from the alphabet.
        ''' </summary>
        ''' <param name="CanRemove">Tells the alphabet if it can remove the Message Code.</param>
        Private Sub RemovingMessageCodeAlphabet(ByRef CanRemove As Boolean) Handles __Alphabet.RemovingMessageCode
            CanRemove = (__DataStream Is Nothing) OrElse (__Chunk = 0)
        End Sub

        ''' <summary>
        ''' (Private)
        ''' Read a chunk from the DataStream and process it, generating thus the Huffman Tree
        ''' and then, generates the Huffman Codes from it.
        ''' </summary>
        Private Sub BuildTable()

            Dim lngTotalRead As Long        ' <-- Total of bytes read from the stream
            Dim intBytesRead As Integer     ' <-- Number of bytes read from the stream
            Dim abytMessage() As Byte = {}  ' <-- Temporary Buffer for message
            Dim lngCountMessages As Long    ' <-- Total of Messages
            Dim strMessage As String        ' <-- Message converted in string
            Dim oNode As zlibVBNET.Huffman.Node  ' <-- Auxiliary Node
            Dim Nodes As New NodeCollection ' <-- Nodes of the tree
            Dim dicCount As New System.Collections.Specialized.NameValueCollection

            '*
            '* If we are buinding a table for another chunk, we clear the table first
            '*
            If (__Chunk > 0) Then
                Nodes.Clear()
                __Table = New zlibVBNET.Huffman.Node
            End If

            '*
            '* Discovers the number of messages
            '*
            __Chunk += 1 ' <-- Increments the chunk counter
            Do
                If (__Alphabet Is Nothing) Then
                    '*
                    '* Using Bytes
                    '*
                    ReDim abytMessage(0)                                ' <-- Clear and prepare the buffer
                    intBytesRead = __DataStream.Read(abytMessage, 0, 1) ' <-- Read the bytes from buffer
                Else
                    ReDim abytMessage(__Alphabet.Item(0).Length - 1)
                    intBytesRead = __DataStream.Read(abytMessage, 0, abytMessage.Length)
                End If
                lngTotalRead += intBytesRead                                        ' <-- Increments the total counter

                '*
                '* Check is something was read
                '*
                If (intBytesRead > 0) Then
                    '*
                    '* Check the MessageCode alphabet
                    '*
                    If (Not __Alphabet Is Nothing) Then
                        If (__Alphabet.IndexOf(abytMessage) = (-1)) Then
                            Dim strAux As String
                            For Each bytByte As Byte In abytMessage
                                strAux &= ", " & bytByte
                            Next
                            Throw New zlibVBNET.Exceptions.MessageCodeNotFound("The Message Code {" & strAux.Substring(2) & "} could not be found in the alphabet supplied.")
                        End If
                    End If

                    '*
                    '* Converts the Message into Strings
                    '*
                    lngCountMessages += 1
                    strMessage = System.Text.BinaryEncoding.GetString(abytMessage)

                    '*
                    '* Check if the message was aready found on the list
                    '*
                    Try
                        '*
                        '* If the node was not found, then add to the collection
                        '*
                        If (dicCount.Item(strMessage) Is Nothing) Then
                            dicCount.Set(strMessage, 1)
                        Else
                            dicCount.Set(strMessage, dicCount.Item(strMessage) + 1)
                        End If
                    Catch ex As Exception
                        Throw
                    End Try

                    '*
                    '* Check if we reached the Chunk Length
                    '*
                    If (__ChunkLength <> (-1)) Then
                        If (lngTotalRead >= ChunkLength) Then
                            Exit Do
                        End If
                    End If
                End If

                '*
                '* Let's be nice, and tell the OS to do something else
                '*
                Application.DoEvents()

                If (__Alphabet Is Nothing) Then
                    If (intBytesRead <= 0) Then
                        Exit Do
                    End If
                Else
                    If (intBytesRead < __Alphabet.Item(0).Length) Then
                        Exit Do
                    End If
                End If
            Loop

            '*
            '* Now we calculate the probabilities of each node
            '*                          
            For Each strMessage In dicCount
                oNode = New zlibVBNET.Huffman.Node(strMessage)
                oNode.SetProbability(Double.Parse(dicCount(strMessage)) / lngCountMessages)
                Nodes.Add(oNode)
            Next

            '*
            '* Then we sort the list
            '*
            Call Nodes.SortNodes()

            '*
            '* Now that we have the Message List assembled
            '* we start with the two last items from the list 
            '* and build ut the table
            '*
            Do While (Nodes.Count > 1)

                '*
                '* We create a new item that points to the last two items of the list
                '*
                oNode = New zlibVBNET.Huffman.Node
                oNode.SetChildLeft(Nodes.Item(Nodes.Count - 2))
                oNode.SetChildRight(Nodes.Item(Nodes.Count - 1))

                '*
                '* The probability of this new added code is the sum of its two children
                '*
                oNode.SetProbability(oNode.ChildLeft.Probability + oNode.ChildRight.Probability)

                '*
                '* Here we already define the codes and the code length for each node
                '*
                oNode.ChildLeft.SetCodeLength(1)
                oNode.ChildRight.SetCodeLength(1)

                oNode.ChildLeft.SetCode(0)
                oNode.ChildRight.SetCode(1)

                '*
                '* Now we remove the last two items
                '*
                Nodes.Remove(Nodes.Count - 1)
                Nodes.Remove(Nodes.Count - 1)

                '*
                '* Finnaly we add the newly created node in the list
                '*
                Nodes.Add(oNode)

                '*
                '* Now we resort the list
                '*
                Nodes.SortNodeUp(Nodes.Count - 1)

                '*
                '* Let's be nice 
                '*
                Application.DoEvents()
            Loop

            '*
            '* The table is the last remaining item of the list
            '*
            __Table = Nodes.Item(0)

            '*
            '* Generate the CodeTable
            '*
            Call GenerateCodeTable(__Table)

        End Sub

        ''' <summary>
        ''' Generates a code table based on the Huffman Tree.
        ''' </summary>
        ''' <param name="TableRoot">A zlibVBNET.Huffman.Node object that represents a Huffman Tree from wich the Huffman Codes will be generated.</param>
        Public Sub GenerateCodeTable(ByVal TableRoot As zlibVBNET.Huffman.Node)

            Static intDeep As Integer ' <--  How deep are we?
            Static dicCodes As System.Collections.Specialized.NameValueCollection

            '*
            '* Create the collection on the first call
            '*
            If (intDeep = 0) Then
                dicCodes = New System.Collections.Specialized.NameValueCollection
                __Values = New CodeValueCollection
            End If

            intDeep += 1

            '*
            '* Check if the node has a value
            '*
            If (Not TableRoot.Value Is Nothing) Then
                dicCodes.Set(TableRoot.CodeLength.ToString("000"), IsNothing(dicCodes.Item(TableRoot.CodeLength.ToString("000")), "") & "," & TableRoot.Code)
                __Values.Set(TableRoot.BinaryCode, TableRoot.Value)
            End If

            '*
            '* Build it recursively
            '*
            If (Not TableRoot.ChildLeft Is Nothing) Then
                Call GenerateCodeTable(TableRoot.ChildLeft)
            End If

            If (Not TableRoot.ChildRight Is Nothing) Then
                Call GenerateCodeTable(TableRoot.ChildRight)
            End If

            intDeep -= 1

            '*
            '* Did we finished?
            '*
            If (intDeep = 0) Then
                '*
                '* Transfers the codes to the definitive array
                '*
                __Codes = New CodeLengthCollection
                For Each strCodeLength As String In dicCodes
                    __Codes.Set(strCodeLength, dicCodes.Item(strCodeLength).Substring(1).Split(","))
                Next
            End If
        End Sub

    End Class

End Namespace