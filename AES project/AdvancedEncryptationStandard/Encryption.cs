using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//

// Y SON LAS COLUMNAS
// X SON LAS FILAS

//-------ESTRUCTURA------- (x,y)
//      y
//   x  0   1       2       3       4
//      1 (1,1)   (2,1)   (2,1)   (4,1)
//      2 (1,2)   (2,2)   (2,2)   (4,2)
//      3 (1,3)   (2,3)   (2,3)   (4,3)
//      4 (1,4)   (2,4)   (2,4)   (4,4)

//-------ESTRUCTURA------- (x,y)
//      y
//   x  0   1   2   3   4
//      1   b1  b5  b9  b13
//      2   b2  b6  b10 b14
//      3   b3  b7  b11 b15
//      4   b4  b8  b12 b16

namespace AdvancedEncryptationStandard
{
    public class Encryption
    {
        Message Message;

        public Encryption(string Text, string Key)
        {
            Message = new Message(Text, Key);
        }

        public void Encrypt()
        {
            KeyExpansion();
        }

        #region KeyMethods

        void KeyExpansion()
        {
            for (int i = 0; i < Message.SubKeys.Length; i++)
            {
                Message.SubKeys[i] = new Block();
                //GENERA LA COLUMNA PILAR PARA LA SUBKEY
                Message.SubKeys[i] = RotWord(Message.BlockKey);
                Message.SubKeys[i] = SubBytes(Message.SubKeys[i], 1);
                Message.SubKeys[i] = RCON(Message.BlockKey, Message.SubKeys[i]);
            }
        }

        Block RotWord(Block block)
        {
            Block Subkey = new Block();

            Subkey.BlockArray[0, 1] = block.BlockArray[0, 0];
            Subkey.BlockArray[0, 2] = block.BlockArray[0, 1];
            Subkey.BlockArray[0, 3] = block.BlockArray[0, 2];
            Subkey.BlockArray[0, 0] = block.BlockArray[0, 3];

            return Subkey;
        }

        Block RCON(Block key, Block subkey)
        {
            //XOR ENTRE EL STATE COLUMN Y LA COLUMNA W[i-4]
            for (int i = 0; i < 4; i++)
            {
                subkey.BlockArray[0, i] = (byte)(subkey.BlockArray[0, i] ^ key.BlockArray[0, i]);
                subkey.BlockArray[0, i] = (byte)(subkey.BlockArray[0, i] ^ Tables.RCON[0, 0]);
            }

            //XOR ENTRE W[i-4] con w[i]
            for (int i = 0; i < 3; i++)
            {
                subkey.BlockArray[1, i] = (byte)(subkey.BlockArray[1, i] ^ key.BlockArray[1, i]);
                subkey.BlockArray[2, i] = (byte)(subkey.BlockArray[2, i] ^ key.BlockArray[2, i]);
                subkey.BlockArray[3, i] = (byte)(subkey.BlockArray[3, i] ^ key.BlockArray[3, i]);
            }

            return subkey;
        }
        #endregion

        #region MyRegion

        void AddRoundKey()
        {

        }

        Block SubBytes(Block block, int use)
        {
            switch (use)
            {
                //CASO PARA LAS LLAVES
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        string temp = Convert.ToString(block.BlockArray[0, i]);                       
                        block.BlockArray[0, i] = Tables.SBox[Convert.ToInt32((Convert.ToByte(temp.Substring(1,1)))), Convert.ToInt32((Convert.ToByte(temp.Substring(0, 1))))];
                    }
                    break;
                //CASO PARA EL TEXTO
                case 2:
                    break;
            }

            return block;
        }

        void ShiftRows()
        {

        }

        void MixColumns()
        {

        }

        #endregion
    }

    public class Decryption
    {
        public Decryption(string Text, string Key)
        {

        }
    }

    public class Message
    {
        public string Text { get; set; }
        public string Key { get; set; }
        public string EncryptedText { get; set; }
        public string EncryptedKey { get; set; }

        public Block BlockKey { get; set; }
        public Block[] TextBlocks { get; set; }
        public Block[] SubKeys { get; set; }

        public Message(String text, string key)
        {
            Text = text;
            Key = key;

           if (Text.Length % 16 != 0)
                TextBlocks = new Block[(Text.Length / 16) + 1];
            else
                TextBlocks = new Block[(Text.Length / 16)];

            switch (Key.Length)
            {
                case 16:
                    SubKeys = new Block[10];
                    BlockKey = new Block(4);
                    break;
                case 24:
                    SubKeys = new Block[12];
                    BlockKey = new Block(6);
                    break;
                case 32:
                    SubKeys = new Block[14];
                    BlockKey = new Block(8);
                    break;
            }

            //CONVIERTE EL TEXTO A HEXADECIMAL EN BLOQUES DE 4X4 BYTES
            int c = 0;
            for (int i = 0; i < TextBlocks.Length; i++)
            {
                TextBlocks[i] = new Block();
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (c != Text.Length)
                        {
                            Char C = Convert.ToChar(Text.Substring(c, 1));
                            string temp = Convert.ToString(C, 2).PadLeft(8, '0');
                            byte b = Convert.ToByte(temp, 2);
                            TextBlocks[i].BlockArray[x, y] = b;
                        }
                        else
                            TextBlocks[i].BlockArray[x, y] = Convert.ToByte("");
                        c++;
                    }
                }
            }

            //CONVIERTE LA LLAVE A HEXADECIMAL EN BLOQUES DE (COLUMNS)X4 BYTES
            c = 0;
            for (int x = 0; x < BlockKey.Columns; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Char C = Convert.ToChar(Key.Substring(c, 1));
                    string temp = Convert.ToString(C, 2).PadLeft(8, '0');
                    byte b = Convert.ToByte(temp, 2);
                    BlockKey.BlockArray[x, y] = b;
                    c++;
                }
            }
        }
    }

    public class Block
    {
        public byte[,] BlockArray { get; set; }
        public int Columns { get; set; }

        public Block()
        {
            BlockArray = new byte[4, 4];
            Columns = 4;
        }

        public Block(int columns)
        {
            BlockArray = new byte[columns, 4];
            Columns = columns;
        }

        byte[] ConvertStringToBin(string block)
        {
            int Count = 0;
            byte[] binary = new byte[64];
            foreach (char C in block)
            {
                string temp = Convert.ToString(C, 2).PadLeft(8, '0');
                for (int i = 0; i < 8; i++)
                {
                    binary[Count] = Convert.ToByte(temp.Substring(i, 1));
                    Count++;
                }
            }
            return binary;
        }

        string ConvertBinToString(byte[] binary)
        {
            return Encoding.UTF8.GetString(binary);
        }
    }

    public static class Tables
    {
        public static byte[,] SBox = new byte[16, 16] 
        {
            {0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76},
            {0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0},
            {0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15},
            {0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27, 0xB2, 0x75},
            {0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84},
            {0x53, 0xD1, 0x00, 0xED, 0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF},
            {0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8},
            {0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2},
            {0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73},
            {0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB},
            {0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79},
            {0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08},
            {0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B, 0xBD, 0x8B, 0x8A},
            {0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E},
            {0xE1, 0xF8, 0x98, 0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF},
            {0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16}
        };

        public static byte[,] RCON = new byte[4, 10]
        {
            {0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36},
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00},
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}
        };
    }
}
