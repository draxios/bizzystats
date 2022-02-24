Option Explicit On

Imports System.IO
'Imports Excel = Microsoft.Office.Interop.Excel

Public Class frmMain
    Dim currentLogsFound As New ArrayList
    Dim currentStatsFound As New ArrayList

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbFilter.SelectedIndex = 1
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        fd1.ShowDialog()

        txtLogFolder.Text = fd1.SelectedPath
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        If txtLogFolder.Text = "" Then
            btnSearch.Enabled = False
            Exit Sub
        End If

        currentLogsFound.Clear()
        currentLogsFound = FindLogs(txtLogFolder.Text)
        status.Text = "Loaded " & currentLogsFound.Count & " log files"

        If currentLogsFound.Count > 0 Then
            btnSearch.Enabled = True
        Else
            btnSearch.Enabled = False
        End If

    End Sub

    Private Function FindLogs(folderName As String) As ArrayList

        Dim tempLogFileList As New ArrayList

        If folderName = "" Then Return tempLogFileList

        Dim diLogs As New DirectoryInfo(folderName)
        Dim fiLogs As FileInfo() = diLogs.GetFiles()

        Dim friLog As FileInfo
        For Each friLog In fiLogs
            tempLogFileList.Add(friLog.FullName)
        Next friLog

        Return tempLogFileList
    End Function

    Private Function FindStats2(WhatToFind As String, filename As String) As ArrayList

        Dim tempStats As New ArrayList
        Dim fResult As Boolean = False

        Dim tempFileStrings() As String = File.ReadAllLines(filename)
        Dim whatToFindSplit() As String = WhatToFind.Split(" ")

        For Each Line As String In tempFileStrings

            For Each WhatToFindString As String In whatToFindSplit

                If Line.Contains(WhatToFindString) = True Then
                    fResult = True
                Else
                    fResult = False
                    Exit For
                End If

            Next

            If fResult = True Then tempStats.Add(Line)

        Next

        Return tempStats
    End Function

    Private Function FindStatsOR(WhatToFind As String, filename As String) As ArrayList

        Dim tempStats As New ArrayList

        Dim tempFileStrings() As String = File.ReadAllLines(filename)
        Dim whatToFindSplit() As String = WhatToFind.Split(" ")

        For Each WhatToFindString As String In whatToFindSplit

            For Each Line As String In tempFileStrings
                If Line.Contains(WhatToFindString) = True Then
                    tempStats.Add(Line)
                End If
            Next

        Next

        Return tempStats
    End Function

    Private Function FindStats(WhatToFind As String, filename As String) As ArrayList

        Dim tempStats As New ArrayList

        For Each Line As String In File.ReadLines(filename)
            If Line.Contains(WhatToFind) = True Then
                tempStats.Add(Line)
            End If
        Next

        Return tempStats
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If txtOutput.Text = "" Then Exit Sub

        sd1.FileName = ""
        sd1.ShowDialog()

        If sd1.FileName = "" Then Exit Sub

        txtOutput.SaveFile(sd1.FileName)
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        If currentLogsFound.Count < 1 Then Exit Sub
        If cmbFilter.Text = "" Then Exit Sub

        txtOutput.Clear()
        currentStatsFound.Clear()
        Dim tempStats As New ArrayList

        For Each logFile As String In currentLogsFound
            'get all the found lines
            tempStats = FindStats2(cmbFilter.Text, logFile)

            'clean the output here

            For Each tempStat As String In tempStats
                currentStatsFound.Add(tempStat)
                txtOutput.AppendText(tempStat & vbNewLine)
            Next
        Next

    End Sub

    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        On Error Resume Next
        txtOutput.Width = Me.Width - 43
        txtOutput.Height = Me.Height - 124
    End Sub
End Class
