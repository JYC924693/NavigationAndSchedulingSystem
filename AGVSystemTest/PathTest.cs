using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemTest
{
    [TestClass]
    public class PathTest
    {
        [TestMethod]
        public void F()
        {
            // Specify the data source.
            int[] scores = [97, 50, 88, 92, 32, 80, 81, 60];

            // Define the query expression.
            var scoreQuery =
                from score in scores
                where score > 80
                orderby score descending
                select $"The score is {score}";

            // Execute the query.
            foreach (var i in scoreQuery)
            {
                Console.WriteLine(i);
            }
        }
    }
}
