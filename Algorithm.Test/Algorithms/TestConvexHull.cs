using Algorithms.Algorithms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Algorithm.Test.Algorithms
{
    [TestFixture]
    public class TestConvexHull
    {
        /// <summary>
        /// Test Case
        /// </summary>
        public class TestCase
        {
            public List<Point> Points { get; }
            public List<Point> Expect { get; }

            public TestCase(List<Point> points, List<Point> expect)
            {
                // Points의 순서를 섞어서 Test
                Points = points.Shuffle().ToList();
                Expect = expect;
            }
        }

        private static IEnumerable<TestCase> TestCases
        {
            get
            {
                yield return TestCase1;
                yield return TestCase2;
                yield return TestCase3;
                yield return TestCase4;
                yield return TestCase5;
            }
        }

        /// <summary>
        /// 사각형에 대한 Test Case
        /// </summary>
        private static TestCase TestCase1
        {
            get
            {
                return new TestCase(
                    CreatePoints((1, 1), (172, 1), (172, 287), (1, 287)),
                    CreatePoints((1, 1), (172, 1), (172, 287), (1, 287)));
            }
        }

        /// <summary>
        /// ㄴ 모양의 형상에 대한 Test Case
        /// </summary>
        private static TestCase TestCase2
        {
            get
            {
                return new TestCase(
                    CreatePoints((0, 0), (2, 0), (2, 1), (1, 1), (1, 2), (0, 2)),
                    CreatePoints((0, 0), (2, 0), (2, 1), (1, 2), (0, 2)));
            }
        }

        private static TestCase TestCase3
        {
            get
            {
                return new TestCase(
                    CreatePoints((0, 0), (1, 0), (1, 1), (2, 1), (2, 2), (3, 2), (3, 0)),
                    CreatePoints((0, 0), (3, 0), (3, 2), (2, 2), (1, 1)));
            }
        }

        private static TestCase TestCase4
        {
            get
            {
                return new TestCase(
                    CreatePoints((0, 0), (1, 1)),
                    CreatePoints((0, 0), (1, 1)));
            }
        }

        private static TestCase TestCase5
        {
            get
            {
                var left = -123;
                var right = 172;
                var top = 287;
                var bottom = -288;

                var points = CreatePoints((left, bottom), (right, bottom), (right, top), (left, top));

                // 형상 내부에 존재할 임의의 점 생성
                var random = new Random();
                for (int i = 0; i < 100; i++)
                {
                    var randX = random.Next(left + 1, right - 1);
                    var randY = random.Next(bottom + 1, top - 1);

                    points.Add(new Point(randX, randY));
                }

                return new TestCase(points, CreatePoints((left, bottom), (right, bottom), (right, top), (left, top)));
            }
        }

        private static List<Point> CreatePoints(params (double, double)[] pairs)
          => pairs.Select(p => new Point(p.Item1, p.Item2)).ToList();

        [Test, TestCaseSource(nameof(TestCases))]
        public void TestCreateConvexHull(TestCase testCase)
        {
            // when
            var actual = ConvexHull.Create(testCase.Points, p => p);
            var expect = testCase.Expect;

            // then
            Assert.AreEqual(actual.Count, expect.Count);
            Assert.IsTrue(actual.ScrambledEqual(expect));
        }

        [Test]
        public void Test()
        {
            var origin = new Point(5, 5);
            var pt1 = new Point(0, 0);
            var pt2 = new Point(7, 0);

            var vector1 = pt1 - origin;
            var vector2 = pt2 - origin;

            var angle1 = Vector.AngleBetween(vector1, vector2);
            var angle2 = Vector.AngleBetween(vector2, vector1);

            Assert.IsTrue(true);
        }
    }
}
