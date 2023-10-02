using Robot.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace KorolchukVolodymyr.RobotChallenge
{
    public class KorolchukVolodymyrAlgorithm : IRobotAlgorithm
    {
        public string Author => "Korolchuk Volodymyr";

        public bool IsStationFree(Robot.Common.Robot currentRobot, IList<Robot.Common.Robot> robots, Position stationPosition)
        {
            foreach (var robot in robots)
            {
                if (robot == currentRobot) continue;
                if (robot.Position == stationPosition) return false;
            }

            return true;
        }

        public int CalculateDistance(Position a, Position b)
        {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public int CalculateEnergyConsumption(Position a, Position b)
        {
            return (int)(Math.Pow(b.X - a.X, 2.0) + Math.Pow(b.Y - a.Y, 2.0));
        }

        public bool IsCreationAdvisable(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            int minX = currentRobot.Position.X - 20 <= 0 ? 0: currentRobot.Position.X - 20;
            int minY = currentRobot.Position.Y - 20 <= 0 ? 0 : currentRobot.Position.Y - 20;
            int maxX = currentRobot.Position.X + 20 >= 99 ? 99 : currentRobot.Position.X + 20;
            int maxY = currentRobot.Position.Y + 20 >= 99 ? 99 : currentRobot.Position.Y + 20;

            if (robots.Where(x => x.OwnerName == Author).Count() > 60) return false;

            foreach (var station in map.Stations)
            {
                if (IsStationFree(currentRobot, robots, station.Position))
                {
                    if (station.Position.X <= maxX && station.Position.X >= minX && 
                        station.Position.Y <= maxY && station.Position.Y >= minY)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public EnergyStation FindNearestAvailableStation(Robot.Common.Robot robot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearestStation = null;
            int minCost = int.MaxValue;
            
            foreach (var station in map.Stations)
            {
                if (IsStationFree(robot, robots, station.Position))
                {
                    int cost = CalculateEnergyConsumption(robot.Position, station.Position);

                    if (cost < minCost)
                    {
                        minCost = cost;
                        nearestStation = station;
                    }
                }
            }

            return nearestStation == null ? null : nearestStation;
        }

        public Position CalculateOptimalStep(Robot.Common.Robot currentRobot, IList<Robot.Common.Robot> robots, EnergyStation station)
        {
            int energyConsumption = CalculateEnergyConsumption(currentRobot.Position, station.Position);
            Position pos = station.Position;

            if (robots.Where(x => x.OwnerName == Author).Count() > 30
                && energyConsumption >= currentRobot.Energy * 1.1)
            {
                return currentRobot.Position;
            }

            int iter = 1;
            while(energyConsumption >= currentRobot.Energy)
            {
                iter++;
                if (iter > 50)
                {
                    if (currentRobot.Position.X < station.Position.X)
                    {
                        pos.X = currentRobot.Position.X + 1;
                    }
                    else
                    {
                        pos.X = currentRobot.Position.X - 1;
                    }

                    if (currentRobot.Position.Y < station.Position.Y)
                    {
                        pos.Y = currentRobot.Position.Y + 1;
                    }
                    else
                    {
                        pos.Y = currentRobot.Position.Y - 1;
                    }
                    break;
                }

                if (currentRobot.Position.X < station.Position.X)
                {
                    pos.X = currentRobot.Position.X + (pos.X - currentRobot.Position.X) / 2;
                }
                else
                {
                    pos.X = currentRobot.Position.X - (currentRobot.Position.X - pos.X) / 2;
                }

                if (currentRobot.Position.Y < station.Position.Y)
                {
                    pos.Y = currentRobot.Position.Y + (pos.Y - currentRobot.Position.Y) / 2;
                }
                else
                {
                    pos.Y = currentRobot.Position.Y - (currentRobot.Position.Y - pos.Y) / 2;
                }

                energyConsumption = CalculateEnergyConsumption(currentRobot.Position, pos);
            }

            return pos;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot currentRobot = robots[robotToMoveIndex];

            if ((currentRobot.Energy >= 250) && IsCreationAdvisable(currentRobot, map, robots))
            {
                return new CreateNewRobotCommand();
            }

            EnergyStation station = FindNearestAvailableStation(currentRobot, map, robots);

            if (currentRobot.Position == station.Position)
            {
                return new CollectEnergyCommand();
            }
            else
            {
                var newPosition = CalculateOptimalStep(currentRobot, robots, station);
                if (newPosition == currentRobot.Position)
                    return new CollectEnergyCommand();
                
                return new MoveCommand() { NewPosition =  newPosition };
            }
        }
    }
}


