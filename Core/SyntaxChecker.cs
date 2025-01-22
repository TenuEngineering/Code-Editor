using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Tester
{

    public class SyntaxChecker
    {

        public List<(int startLine, int startColumn, int endColumn, string errorCode, string errorMessage)> GetSyntaxErrorPositions(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // Hata tespit et (Diagnostics)
            var diagnostics = tree.GetDiagnostics();
            

            // Eğer hata yoksa boş liste döndür
            if (!diagnostics.Any())
            {
                return new List<(int startLine, int startColumn, int endColumn, string errorCode, string errorMessage)>();
            }

            // Hataları doldurmak için liste
            var errorPositions = new List<(int startLine, int startColumn, int endColumn, string errorCode, string errorMessage)>();

            // Diagnostic'leri listeye ekle
            foreach (var diagnostic in diagnostics)
            {

                var lineSpan = diagnostic.Location.GetLineSpan();
                var startLine = lineSpan.StartLinePosition.Line;
                var startColumn = lineSpan.StartLinePosition.Character;
                var endColumn = lineSpan.EndLinePosition.Character;

                // Hata kodu ve mesajını ekle
                var errorCode = diagnostic.Id;
                var errorMessage = diagnostic.GetMessage();

                // Hata detaylarını listeye ekle
                errorPositions.Add((startLine, startColumn, endColumn, errorCode, errorMessage));
            }

            // Hata pozisyonları ve mesajlarını döndür
            return errorPositions;
        }

       
        public SyntaxTree GetSyntaxTree(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            return tree;
        }

        public void CheckSyntaxIssues(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            // Parantezleri kontrol et
            var missingParentheses = root.DescendantNodes()
                                         .OfType<ParenthesizedExpressionSyntax>()
                                         .Where(node => node.OpenParenToken.IsMissing || node.CloseParenToken.IsMissing);

            foreach (var node in missingParentheses)
            {
                MessageBox.Show($"Eksik parantez: {node.GetLocation().GetLineSpan()}");
            }

            // Süslü parantezleri kontrol et
            var missingBraces = root.DescendantNodes()
                                    .OfType<BlockSyntax>()
                                    .Where(node => node.OpenBraceToken.IsMissing || node.CloseBraceToken.IsMissing);

            foreach (var node in missingBraces)
            {
                MessageBox.Show($"Eksik süslü parantez: {node.GetLocation().GetLineSpan()}");
            }

            // Noktalı virgülleri kontrol et
            var missingSemicolons = root.DescendantNodes()
                                        .OfType<ExpressionStatementSyntax>()
                                        .Where(node => node.SemicolonToken.IsMissing);

            foreach (var node in missingSemicolons)
            {
                MessageBox.Show($"Eksik noktalı virgül: {node.GetLocation().GetLineSpan()}");
            }
        }


    }

}
