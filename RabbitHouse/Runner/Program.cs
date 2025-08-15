using System.Security.Cryptography.X509Certificates;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));
            foreach (var arrangement in arrangements)
            {
                int count = 0;
                bool changed;
                do
                {
                    changed = false;
                    for (int i = 0; i < arrangement.TotalRows; i++)
                    {
                        for (int j = 0; j < arrangement.TotalColumns; j++)
                        {
                            int current = arrangement[i, j];

                            //right 
                            if (j + 1 < arrangement.TotalColumns)
                            {
                                int right = arrangement[i, j + 1];
                                int diff = Math.Abs(current - right);
                                if (diff > 1)
                                {
                                    if (right > current)
                                    {
                                        arrangement.SetHeightAt(i, j, right - 1);
                                        changed = true;
                                    }
                                    else
                                    {
                                        arrangement.SetHeightAt(i, j + 1, current - 1);
                                        changed = true;
                                    }
                                }
                            }

                            //left 
                            if (j - 1 >= 0)
                            {
                                int left = arrangement[i, j - 1];
                                int diff = Math.Abs(current - left);
                                if (diff > 1)
                                {
                                    if (left > current)
                                    {
                                        arrangement.SetHeightAt(i, j, left - 1);
                                        changed = true;
                                    }
                                    else
                                    {
                                        arrangement.SetHeightAt(i, j - 1, current - 1);
                                        changed = true;
                                    }
                                }
                            }

                            //below 
                            if (i + 1 < arrangement.TotalRows)
                            {
                                int below = arrangement[i + 1, j];
                                int diff = Math.Abs(current - below);
                                if (diff > 1)
                                {
                                    if (below > current)
                                    {
                                        arrangement.SetHeightAt(i, j, below - 1);
                                        changed = true;
                                    }
                                    else
                                    {
                                        arrangement.SetHeightAt(i + 1, j, current - 1);
                                        changed = true;
                                    }
                                }
                            }

                            //above
                            if (i - 1 >= 0)
                            {
                                int above = arrangement[i - 1, j];
                                int diff = Math.Abs(current - above);
                                if (diff > 1)
                                {
                                    if (above > current)
                                    {
                                        arrangement.SetHeightAt(i, j, above - 1);
                                        changed = true;
                                    }
                                    else
                                    {
                                        arrangement.SetHeightAt(i - 1, j, current - 1);
                                        changed = true;
                                    }
                                }
                            }
                        }
                        arrangement.Visualise();
                    }
                    Console.WriteLine(arrangement.GetTotalAddedBlocks());
                } while (changed);
            }
        }
    }
}
