using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;
using System.Collections.Generic;

namespace KorolchukVolodymyr.RobotChallenge.Test
{
    [TestClass]
    public class TestRobotChallenge
    {

        [TestMethod]
        public void TestCalculateEnergyConsumption()
        {
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var pos1 = new Position(55, 19);
            var pos2 = new Position(72, 15);

            var consumption = algorithm.CalculateEnergyConsumption(pos1, pos2);
            Assert.AreEqual(consumption, 305);
        }

        [TestMethod]
        public void TestCalculateDistance() 
        {
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var pos1 = new Position(2, 2);
            var pos2 = new Position(4, 4);

            var distance = algorithm.CalculateDistance(pos1, pos2);
            Assert.AreEqual(distance, 8);
        }

        [TestMethod]
        public void TestIsStationFree()
        {
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var map = new Map();
            var robots = new List<Robot.Common.Robot>()
            {
                new Robot.Common.Robot() { Energy = 100, Position = new Position(9, 4) },
                new Robot.Common.Robot() { Energy = 80, Position = new Position(5, 7) }
            };
            var stationPosition = new Position(9, 4);

            map.Stations.Add(new EnergyStation() { Energy = 50, Position = stationPosition });

            bool isStationFree =  algorithm.IsStationFree(robots[0], robots, stationPosition);

            Assert.IsTrue(isStationFree);
        }

        [TestMethod]
        public void TestMoveCommand()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() { Energy = 100, Position = new Position(21, 15)} };
            var stationPosition = new Position(20, 20);

            map.Stations.Add(new EnergyStation()
            {
                Energy = 50,
                Position = stationPosition
            });

            var command = algorithm.DoStep(robots, 0, map);

            Assert.IsTrue(command is MoveCommand);
            Assert.AreEqual(((MoveCommand)command).NewPosition, stationPosition);
        }
        
        [TestMethod]
        public void TestCollectEnergyCommand()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>() 
            { 
                new Robot.Common.Robot() 
                { 
                    Energy = 100, 
                    Position = new Position(30, 30) 
                } 
            };

            map.Stations.Add(new EnergyStation()
            {
                Energy = 50,
                Position = new Position(30, 30)
            });

            var command = algorithm.DoStep(robots, 0, map);

            Assert.IsTrue(command is CollectEnergyCommand);
        }

        [TestMethod]
        public void TestCreateNewRobotCommand()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();

            var robots = new List<Robot.Common.Robot>() 
            { 
                new Robot.Common.Robot() 
                { 
                    Energy = 350, 
                    Position = new Position(0, 0) 
                } 
            };

            map.Stations.Add(new EnergyStation()
            {
                Energy = 50,
                Position = new Position(20, 20)
            });

            var command = algorithm.DoStep(robots, 0, map);

            Assert.IsTrue(command is CreateNewRobotCommand);
        }

        [TestMethod]
        public void TestFindNearestAvailableStationOne()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>()
            {
                new Robot.Common.Robot() { Energy = 100, Position = new Position(9, 4) },
                new Robot.Common.Robot() { Energy = 100, Position = new Position(10, 5) },
                new Robot.Common.Robot() { Energy = 100, Position = new Position(89, 32) },
            };

            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(10, 5) });
            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(89, 32) });
            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(99, 99) });

            var nearestStation = algorithm.FindNearestAvailableStation(robots[0], map, robots);

            Assert.AreEqual(map.Stations[2], nearestStation);
        }  
        
        [TestMethod]
        public void TestFindNearestAvailableStationTwo()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>()
            {
                new Robot.Common.Robot() { Energy = 100, Position = new Position(0, 0) },
                new Robot.Common.Robot() { Energy = 100, Position = new Position(10, 5) },
                new Robot.Common.Robot() { Energy = 100, Position = new Position(89, 32) },
                new Robot.Common.Robot() { Energy = 100, Position = new Position(99, 99) },
            };

            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(10, 5) });
            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(89, 32) });
            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(99, 99) });

            var nearestStation = algorithm.FindNearestAvailableStation(robots[0], map, robots);

            Assert.AreEqual(null, nearestStation);
        }    
        
        [TestMethod]
        public void TestCalculateOptimalStep()
        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>()
            {
                new Robot.Common.Robot() { Energy = 100, Position = new Position(0, 0) },
            };

            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(99, 99) });

            var newPosition = algorithm.CalculateOptimalStep(robots[0], robots, map.Stations[0]);

            Assert.AreNotEqual(robots[0].Position, newPosition);
        }

        [TestMethod]
        public void TestIsCreationAdvisable()

        {
            var map = new Map();
            var algorithm = new KorolchukVolodymyrAlgorithm();
            var robots = new List<Robot.Common.Robot>()
            {
                new Robot.Common.Robot() { Energy = 100, Position = new Position(25, 25) },
            };

            map.Stations.Add(new EnergyStation() { Energy = 50, Position = new Position(50, 50) });
            map.Stations.Add(new EnergyStation() { Energy = 70, Position = new Position(99, 99) });
            map.Stations.Add(new EnergyStation() { Energy = 20, Position = new Position(30, 30) });
            map.Stations.Add(new EnergyStation() { Energy = 40, Position = new Position(0, 0) });

            var isCreationAdvisable = algorithm.IsCreationAdvisable(robots[0], map, robots);

            Assert.IsTrue(isCreationAdvisable);
        }

    }
}
