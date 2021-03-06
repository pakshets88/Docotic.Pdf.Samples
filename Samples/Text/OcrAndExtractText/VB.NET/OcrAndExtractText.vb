Imports System.IO
Imports System.Text
Imports Tesseract

Namespace BitMiracle.Docotic.Pdf.Samples
    Public NotInheritable Class OcrAndExtractText
        Public Shared Sub Main()
            Dim documentText = New StringBuilder()

            Using pdf = New PdfDocument("Sample data/Freedman Scora.pdf")

                Using engine = New TesseractEngine("tessdata", "eng", EngineMode.[Default])

                    For i As Integer = 0 To pdf.PageCount - 1
                        If documentText.Length > 0 Then documentText.Append(vbCrLf & vbCrLf)
                        Dim page As PdfPage = pdf.Pages(i)
                        Dim searchableText As String = page.GetText()

                        If Not String.IsNullOrEmpty(searchableText.Trim()) Then
                            documentText.Append(searchableText)
                            Continue For
                        End If

                        Dim options As PdfDrawOptions = PdfDrawOptions.Create()
                        options.BackgroundColor = New PdfRgbColor(255, 255, 255)
                        options.HorizontalResolution = 600
                        options.VerticalResolution = 600
                        Dim pageImage As String = $"page_{i}.png"
                        page.Save(pageImage, options)

                        Using img = Pix.LoadFromFile(pageImage)

                            Using recognizedPage = engine.Process(img)
                                Dim recognizedText = recognizedPage.GetText()
                                Console.WriteLine($"Mean confidence for page #{i}: {recognizedPage.GetMeanConfidence()}")
                                documentText.Append(recognizedText)
                            End Using
                        End Using
                    Next
                End Using

                Const Result As String = "result.txt"

                Using writer = New StreamWriter(Result)
                    writer.Write(documentText.ToString())
                End Using

                Process.Start(Result)
            End Using
        End Sub
    End Class
End Namespace