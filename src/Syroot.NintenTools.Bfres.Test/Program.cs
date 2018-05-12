using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.Bfres.Helpers;
using Syroot.NintenTools.Yaz0;

namespace Syroot.NintenTools.Bfres.Test
{
    internal class Program
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private static Stopwatch _stopwatch = new Stopwatch();
        private static string[] _searchPaths = new string[]
        {
            @"D:\Pictures\zBFRES\Decompressed",
            @"D:\Archive\Wii U\_Roms\MK8"
        };
        private static StreamWriter _log = new StreamWriter(File.OpenWrite(@"D:\Pictures\log.txt"));

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            Yaz0Compression.Decompress(@"D:\Pictures\zBFRES\BotW\Animal_Bass.sbfres",
                @"D:\Pictures\zBFRES\BotW\Animal_Bass.bfres");
            ResFile resFile = new ResFile(@"D:\Pictures\zBFRES\BotW\Animal_Bass.bfres");

            //ResFile resFile = new ResFile(@"D:\Archive\Wii U\_Roms\MK8\content\race_common\Coin\Coin.bfres");
            //resFile.Models[0].Shapes[0].Meshes[0].SetIndices(new uint[] { 1, 2, 3 }, GX2IndexFormat.UInt32);
            //resFile.Save(@"D:\Archive\Wii U\_Roms\MK8\content\race_common\Coin\Coin.bfres");

            //LoadResFiles(LogSurfaceFormats);
            
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
        
        private static void TestVertexAttribFormat(ResFile resFile)
        {
            foreach (Model model in resFile.Models.Values)
            {
                int vertexBufferIndex = 0;
                foreach (VertexBuffer vertexBuffer in model.VertexBuffers)
                {
                    Console.WriteLine($"\tVertexBuffer {vertexBufferIndex}");
                    VertexBufferHelper helper = new VertexBufferHelper(vertexBuffer, ByteOrder.LittleEndian);
                    foreach (VertexBufferHelperAttrib attrib in helper.Attributes)
                    {
                        IList<Vector4F> elements;
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (BinaryDataWriter writer = new BinaryDataWriter(stream, true))
                            {
                                writer.Write(attrib.Data, attrib.Format);
                            }
                            stream.Position = 0;
                            using (BinaryDataReader reader = new BinaryDataReader(stream, true))
                            {
                                elements = reader.ReadGX2Attribs(attrib.Data.Length, attrib.Format);
                            }
                        }
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (elements[i] != attrib.Data[i])
                            {
                                Console.WriteLine($"\t\tError {attrib.Name} {attrib.Format}");
                                break;
                            }
                        }
                    }
                    vertexBufferIndex++;
                }
            }
        }

        private static void LoadResFiles(Action<string, ResFile> fileAction = null)
        {
            foreach (string searchPath in _searchPaths)
            {
                foreach (string fileName in Directory.GetFiles(searchPath, "*.bfres", SearchOption.AllDirectories))
                {  
                    Console.Write($"Loading {fileName}...");

                    _stopwatch.Restart();
                    ResFile resFile = new ResFile(fileName);
                    _stopwatch.Stop();
                    Console.WriteLine($" {_stopwatch.ElapsedMilliseconds}ms");

                    fileAction?.Invoke(fileName, resFile);
                }
            }
        }
    }
}
