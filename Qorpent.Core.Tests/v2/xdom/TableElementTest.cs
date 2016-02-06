using System;
using NUnit.Framework;
using qorpent.v2.xdom;
using Qorpent.Utils.Extensions;
using static qorpent.v2.xdom.XDom;
namespace Qorpent.Core.Tests.v2.xdom {
    [TestFixture]
    public class TableElementTest {
        [Test]
        public void SimpleBuild() {

            var t = html(
                h1("Тут таблица"),
                table().setClass("xxx")
                    .headrow("Name",th("A").colspan(2),"B","C")
                    .row("X",1,2,3)
                    .row("y",1,2,3)
                );

            var res = t.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(res);
            Assert.AreEqual(@"<html>
  <head>
    <meta charset='utf-8' />
  </head>
  <body>
    <h1>Тут таблица</h1>
    <table class='xxx'>
      <thead>
        <tr>
          <th>Name</th>
          <th colspan='2'>A</th>
          <th>B</th>
          <th>C</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>X</td>
          <td>1</td>
          <td>2</td>
          <td>3</td>
        </tr>
        <tr>
          <td>y</td>
          <td>1</td>
          <td>2</td>
          <td>3</td>
        </tr>
      </tbody>
    </table>
  </body>
</html>", res);
        }
    }
}