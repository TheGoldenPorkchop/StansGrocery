'Angel Nava
'Spring 2025
'RCET2265
'Graphics Example
'link
'This code is pretty broken
Option Strict On
Option Explicit On
Imports System.IO
Public Class StansGroceryForm

    Dim item$(,)

    Sub ReadfromFile()
        Dim filePath As String = "..\..\Grocery.txt"
        Dim fileNumber As Integer = FreeFile()
        Dim currentRecord As String = ""
        Dim temp() As String 'use for spliting custom data

        Try
            FileOpen(fileNumber, filePath, OpenMode.Input)

            Do Until EOF(fileNumber)
                Input(fileNumber, currentRecord) 'reads exactly one record
                If currentRecord <> "" Then 'ignores blank records
                    temp = Split(currentRecord, ",")
                    'DisplayListBox.Items.Add(currentRecord)

                    If temp.Length = 4 Then 'ignores corrupted/malformed records
                        temp(0) = Replace(temp(0), "$$ITM", "")
                        temp(1) = Replace(temp(1), "##LOC", "")
                        temp(2) = Replace(temp(2), "%%CAT", "")
                        DisplayListBox.Items.Add(temp(0))
                        WritetoFile(temp(0)) 'Item
                        WritetoFile(temp(1)) 'Aisle
                        WritetoFile(temp(2)) 'Category
                    End If

                End If
            Loop

            FileClose(fileNumber)
        Catch woag As FileNotFoundException
            MsgBox("Woag")
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace & vbNewLine & ex.GetBaseException.ToString)
        End Try
    End Sub

    Sub WritetoFile(newRecord As String, Optional insertLine As Boolean = False)
        Dim filePath As String = "..\..\Grocery.txt"
        Dim fileNumber As Integer = FreeFile()

        Try
            FileOpen(fileNumber, filePath, OpenMode.Append)
            Write(fileNumber, newRecord)

            If insertLine Then
                WriteLine(fileNumber)
            End If

            FileClose(fileNumber)
        Catch ex As Exception
            MsgBox("err")
        End Try

    End Sub

    Sub LoadItemData()
        Dim filePath As String = "..\..\Grocery.txt"
        Dim fileNumber As Integer = FreeFile()
        Dim currentRecord As String
        Dim InvalidFileName As Boolean = True
        Dim _Items(NumberOfItems(filePath) - 1, 2) As String 'array for Item data
        Dim currentItem As Integer = 0

        Do

            Try
                FileOpen(fileNumber, filePath, OpenMode.Input)
                InvalidFileName = False
                Do Until EOF(fileNumber)
                    Input(fileNumber, currentRecord)
                    _Items(currentItem, 0) = currentRecord 'Name of Item
                    Input(fileNumber, currentRecord)
                    _Items(currentItem, 1) = currentRecord 'Aisle
                    Input(fileNumber, currentRecord)
                    _Items(currentItem, 2) = currentRecord 'Category
                    Input(fileNumber, currentRecord) 'empty to discard

                    currentItem += 1

                Loop
                FileClose(fileNumber)
                'MsgBox($"There are {NumberOfItems(filePath)} Items")

            Catch woag As FileNotFoundException
                InvalidFileName = False
                MsgBox($"The current file is {filePath}")

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        Loop While InvalidFileName

        items(_Items)

    End Sub

    Sub DisplayFilteredData()
        Dim _items(,) As String = items()
        FilterComboBox.Items.Clear()
        'Makes sure the array has schtuff
        If _items IsNot Nothing Then
            For row = 0 To _items.GetUpperBound(0) 'check every row
                For column = 0 To _items.GetUpperBound(1) 'check every column
                    If InStr(_items(row, column), SearchTextBox.Text) > 0 Then 'search withing string
                        Select Case True
                            Case FilterByAisleRadioButton.Checked
                                If Not FilterComboBox.Items.Contains(_items(row, 1)) Then 'don't add duplicates
                                    FilterComboBox.Items.Add(_items(row, 1))
                                End If
                            Case FilterByCategoryRadioButton.Checked
                                If Not FilterComboBox.Items.Contains(item$(row, 2)) Then 'don't add duplicates
                                    FilterComboBox.Items.Add(item$(row, 2))
                                End If

                        End Select

                    End If
                Next
            Next

            FilterComboBox.Sorted = True

            If FilterComboBox.Items.Count >= 1 Then 'if there are results Filter the first one
                FilterComboBox.SelectedIndex() = 0
            End If
        End If
    End Sub

    Function NumberOfItems(fileName As String) As Integer
        Dim count As Integer = 0
        Dim filePath As String = "..\..\Grocery.txt"
        Dim fileNumber As Integer = FreeFile()

        Try
            FileOpen(fileNumber, fileName, OpenMode.Input)
            While Not EOF(fileNumber)
                LineInput(fileNumber)
                count += 1
            End While
            FileClose(fileNumber)
        Catch ex As Exception
            'count = -1
        End Try

        Return count
    End Function

    Function items(Optional itemData(,) As String = Nothing) As String(,)
        Static _items(,) As String
        If itemData IsNot Nothing Then
            _items = itemData
        End If
        Return _items
    End Function

    Sub FillListBox(searchColumn As Integer)
        Dim _items(,) As String = items()
        DisplayListBox.Items.Clear()

        If _items IsNot Nothing Then
            For row = 0 To _items.GetUpperBound(0) 'check every row
                If _items(row, searchColumn) = FilterComboBox.SelectedItem.ToString() Then
                    DisplayListBox.Items.Add($"{_items(row, 0) & "," & _items(row, 1).PadRight(25)} {_items(row, 2)}")
                End If
            Next

        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
        ReadfromFile()
    End Sub

    Private Sub StansGrocery_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadItemData()
        ReadfromFile()
    End Sub

    Private Sub SelectComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles FilterComboBox.SelectedIndexChanged
        Select Case True
            Case FilterByAisleRadioButton.Checked
                FillListBox(1)
            Case FilterByCategoryRadioButton.Checked
                FillListBox(2)
        End Select

    End Sub

    Private Sub FilterRadioButtons_CheckedChanged(sender As Object, e As EventArgs) Handles FilterByAisleRadioButton.CheckedChanged, FilterByCategoryRadioButton.CheckedChanged
        DisplayFilteredData()
    End Sub

End Class
