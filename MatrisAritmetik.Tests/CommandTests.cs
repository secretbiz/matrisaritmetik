using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MatrisAritmetik.Tests
{
    [TestClass]
    public class CommandTests
    {
        /// <summary>
        /// Service for command tokenization and evaluation
        /// </summary>
        private static readonly IFrontService FrontService = new FrontService();

        public static CommandInfo InfoSetter(string func,
                                             IFrontService service)
        {
            if (service.TryParseBuiltFunc(func, out CommandInfo cmdinfo))
            { return cmdinfo; }
            else
            { throw new System.Exception("Fonksiyon " + func + " bilgisi alınamadı!"); }
        }

        public static CommandInfo RankInfo = InfoSetter("Rank", FrontService);

        public static CommandInfo TransposeInfo = InfoSetter("Transpose", FrontService);
        /// <summary>
        /// Example matrix that has same name with a function
        /// Transpose = { {1, 2}, {3, 4} }
        /// </summary>
        public static MatrisBase<dynamic> Transpose =
            new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, 2 },
                    new List<dynamic>(){ 3, 4 }
                }
            );

        /// <summary>
        /// Example matrices to use for tests
        /// A = { {1, 2}, {3, 4} }
        /// </summary>
        public MatrisBase<dynamic> A =
            new MatrisBase<dynamic>(new List<List<dynamic>>()
                {
                    new List<dynamic>(){ 1, 2 },
                    new List<dynamic>(){ 3, 4 }
                }
            );

        /// <summary>
        /// No errors should be thrown from these
        /// {Topic : { command, expected_tokens_and_output } }
        /// </summary>
        public Dictionary<string, Dictionary<string, List<Token>>> CMDS = new Dictionary<string, Dictionary<string, List<Token>>>()
        {
            { "Basit_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "1+1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="+"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=2}}
                    },
                    { "2-1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=1}}
                    },
                    { "2*1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=2}}
                    },
                    { "2/1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=2}}
                    },
                    { "2^3", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                               new Token(){tknType=TokenType.NUMBER,val=3},
                                               new Token(){tknType=TokenType.OUTPUT,val=8}}
                    },
                    { "3^-1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=3},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                new Token(){tknType=TokenType.NUMBER,val=1},
                                                new Token(){tknType=TokenType.OUTPUT,val=0.3333333333333333}}
                    },
                    { "2%1", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                }
            },
            { "Unary_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "+1", new List<Token>(){new Token(){tknType=TokenType.OPERATOR,symbol="u+"},
                                              new Token(){tknType=TokenType.NUMBER,val=1},
                                              new Token(){tknType=TokenType.OUTPUT,val=1}}
                    },
                    { "-1", new List<Token>(){new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                              new Token(){tknType=TokenType.NUMBER,val=1},
                                              new Token(){tknType=TokenType.OUTPUT,val=-1}}
                    },
                    { "--1", new List<Token>(){new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                               new Token(){tknType=TokenType.NUMBER,val=1},
                                               new Token(){tknType=TokenType.OUTPUT,val=1}}
                    },
                    { "2-(-1)", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                                  new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                  new Token(){tknType=TokenType.LEFTBRACE},
                                                  new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                  new Token(){tknType=TokenType.NUMBER,val=1},
                                                  new Token(){tknType=TokenType.RIGHTBRACE},
                                                  new Token(){tknType=TokenType.OUTPUT,val=3}}
                    },
                }
            },
            { "Extreme_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "0*2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                               new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "0*0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "0/2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                               new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "2/0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=double.PositiveInfinity}}
                    },
                    { "-2/0", new List<Token>(){new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                new Token(){tknType=TokenType.NUMBER,val=2},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                                new Token(){tknType=TokenType.NUMBER,val=0},
                                                new Token(){tknType=TokenType.OUTPUT,val=double.NegativeInfinity}}
                    },
                    { "2^0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=1}}
                    },
                    { "0^-3", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                new Token(){tknType=TokenType.NUMBER,val=3},
                                                new Token(){tknType=TokenType.OUTPUT,val=double.PositiveInfinity}}
                    },
                    { "0^2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                               new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "0^0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=double.NaN}}
                    },
                    { "0%2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                               new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "2%0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=double.NaN}}
                    },
                    { "0%0", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                               new Token(){tknType=TokenType.NUMBER,val=0},
                                               new Token(){tknType=TokenType.OUTPUT,val=double.NaN}}
                    },
                }
            },
            { "Priority_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "2*3+5", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                 new Token(){tknType=TokenType.NUMBER,val=3},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="+"},
                                                 new Token(){tknType=TokenType.NUMBER,val=5},
                                                 new Token(){tknType=TokenType.OUTPUT,val=11}}
                    },
                    { "5+3*2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=5},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="+"},
                                                 new Token(){tknType=TokenType.NUMBER,val=3},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OUTPUT,val=11}}
                    },
                    { "4*(2-2)", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=4},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                 new Token(){tknType=TokenType.LEFTBRACE},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.RIGHTBRACE},
                                                 new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "(2-2)*4", new List<Token>(){new Token(){tknType=TokenType.LEFTBRACE},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.RIGHTBRACE},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                 new Token(){tknType=TokenType.NUMBER,val=4},
                                                 new Token(){tknType=TokenType.OUTPUT,val=0}}
                    },
                    { "8/2-2", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=8},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                 new Token(){tknType=TokenType.NUMBER,val=2},
                                                 new Token(){tknType=TokenType.OUTPUT,val=2}}
                    }
                }
            },
            { "Associative_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "4/(2-1)", new List<Token>(){new Token(){tknType=TokenType.NUMBER,val=4},
                                                   new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                                   new Token(){tknType=TokenType.LEFTBRACE},
                                                   new Token(){tknType=TokenType.NUMBER,val=2},
                                                   new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                   new Token(){tknType=TokenType.NUMBER,val=1},
                                                   new Token(){tknType=TokenType.RIGHTBRACE},
                                                   new Token(){tknType=TokenType.OUTPUT,val=4}}
                    },
                    { "(2-1)/4", new List<Token>(){new Token(){tknType=TokenType.LEFTBRACE},
                                                   new Token(){tknType=TokenType.NUMBER,val=2},
                                                   new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                   new Token(){tknType=TokenType.NUMBER,val=1},
                                                   new Token(){tknType=TokenType.RIGHTBRACE},
                                                   new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                                   new Token(){tknType=TokenType.NUMBER,val=4},
                                                   new Token(){tknType=TokenType.OUTPUT,val=1.0/4}}
                    }
                }
            },
            { "Matris_Basit_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "A+3", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="+"},
                                                new Token(){tknType=TokenType.NUMBER,val=3},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 4, 5 },
                                                        new List<dynamic>(){ 6, 7 }
                                                    })
                                                }
                                               }
                    },
                    { "A-5", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="-"},
                                                new Token(){tknType=TokenType.NUMBER,val=5},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ -4, -3 },
                                                        new List<dynamic>(){ -2, -1 }
                                                    })
                                                }
                                               }
                    },
                    { "A*3", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                new Token(){tknType=TokenType.NUMBER,val=3},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 3, 6 },
                                                        new List<dynamic>(){ 9, 12 }
                                                    })
                                                }
                                               }
                    },
                    { "A/4", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="/"},
                                                new Token(){tknType=TokenType.NUMBER,val=4},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 1.0/4, 2.0/4 },
                                                        new List<dynamic>(){ 3.0/4, 4.0/4 }
                                                    })
                                                }
                                               }
                    },
                    { "A^2", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                new Token(){tknType=TokenType.NUMBER,val=2},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 1, 4 },
                                                        new List<dynamic>(){ 9, 16 }
                                                    })
                                                }
                                               }
                    },
                    { "A%3", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                                new Token(){tknType=TokenType.NUMBER,val=3},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 1, 2 },
                                                        new List<dynamic>(){ 0, 1 }
                                                    })
                                                }
                                               }
                    },
                }
            },
            { "Matris_Custom_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "A.*A", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol=".*"},
                                                 new Token(){tknType=TokenType.MATRIS,name="A"},
                                                 new Token(){
                                                     tknType=TokenType.OUTPUT,
                                                     val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                     {
                                                         new List<dynamic>(){ 7, 10 },
                                                         new List<dynamic>(){ 15, 22 }
                                                     })
                                                 }
                                               }
                    },
                    { "A%A", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                                new Token(){tknType=TokenType.MATRIS,name="A"},
                                                new Token(){
                                                    tknType=TokenType.OUTPUT,
                                                    val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                    {
                                                        new List<dynamic>(){ 0, 0 },
                                                        new List<dynamic>(){ 0, 0 }
                                                    })
                                                }
                                               }
                    },
                    { "A.^3", new List<Token>(){ new Token(){tknType=TokenType.MATRIS,name="A"},
                                                 new Token(){tknType=TokenType.OPERATOR,symbol=".^"},
                                                 new Token(){tknType=TokenType.NUMBER,val=3},
                                                 new Token(){
                                                     tknType=TokenType.OUTPUT,
                                                     val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                     {
                                                         new List<dynamic>(){ 37, 54 },
                                                         new List<dynamic>(){ 81, 118 }
                                                     })
                                                 }
                                               }
                    },
                }
            },
            { "Matris_Special_Service" ,
                new Dictionary<string,List<Token>>(){
                    { "!Identity(2)", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="Identity"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 1, 0 },
                                                                 new List<dynamic>(){ 0, 1 }
                                                             })
                                                         }
                                                       }
                    },
                    { "!RandIntMat(2,2,0,4,0)", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 1, 4 },
                                                                 new List<dynamic>(){ 1, 1 }
                                                             })
                                                         }
                                                       }
                    },
                    { "!RandIntMat(2,2,0,4)", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.RIGHTBRACE}
                                                       }
                    },
                    { "!RandFloatMat(2,2,0,4,0)", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="RandFloatMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 2.904973 , 3.2693014 },
                                                                 new List<dynamic>(){ 3.0720909, 2.2326448 }
                                                             })
                                                         }
                                                       }
                    },
                    { "!RandFloatMat(2,2,0,4)", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="RandFloatMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.RIGHTBRACE}
                                                       }
                    },
                    { "!Help()", new List<Token>(){ new Token(){tknType=TokenType.FUNCTION,name="Help"},
                                                    new Token(){tknType=TokenType.LEFTBRACE},
                                                    new Token(){tknType=TokenType.RIGHTBRACE},
                                                    new Token(){
                                                        tknType=TokenType.OUTPUT,
                                                        val=CompilerMessage.COMPILER_HELP
                                                    }
                                                  }
                    },
                }
            },

            { "Matris_Combined_Aritmetik" ,
                new Dictionary<string,List<Token>>(){
                    { "!RandIntMat(2,2,0,4,0)^3", new List<Token>(){
                                                         new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                         new Token(){tknType=TokenType.NUMBER,val=3},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 1, 64 },
                                                                 new List<dynamic>(){ 1, 1 }
                                                             })
                                                         }
                                                       }
                    },
                    { "2*!RandIntMat(2,2,0,4,0)^-2", new List<Token>(){
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                         new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 2, 0.125 },
                                                                 new List<dynamic>(){ 2, 2 }
                                                             })
                                                         }
                                                       }
                    },
                    { "(2*!RandIntMat(2,2,0,4,0))^-2", new List<Token>(){
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="*"},
                                                         new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="^"},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="u-"},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 0.25, 1.0/64 },
                                                                 new List<dynamic>(){ 0.25, 0.25 }
                                                             })
                                                         }
                                                       }
                    },
                    { "(!Identity(2)+1.5).*!RandIntMat(2,2,0,4,0)%2", new List<Token>(){
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.FUNCTION,name="Identity"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="+"},
                                                         new Token(){tknType=TokenType.NUMBER,val=1.5},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol=".*"},
                                                         new Token(){tknType=TokenType.FUNCTION,name="RandIntMat"},
                                                         new Token(){tknType=TokenType.LEFTBRACE},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=4},
                                                         new Token(){tknType=TokenType.ARGSEPERATOR},
                                                         new Token(){tknType=TokenType.NUMBER,val=0},
                                                         new Token(){tknType=TokenType.RIGHTBRACE},
                                                         new Token(){tknType=TokenType.OPERATOR,symbol="%"},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){tknType=TokenType.NUMBER,val=2},
                                                         new Token(){
                                                             tknType=TokenType.OUTPUT,
                                                             val=new MatrisBase<dynamic>(new List<List<dynamic>>()
                                                             {
                                                                 new List<dynamic>(){ 0, 1.5 },
                                                                 new List<dynamic>(){ 0, 0.5 }
                                                             })
                                                         }
                                                       }
                    },
                }
            },
            { "Docs" ,
                new Dictionary<string,List<Token>>(){
                    { "?", new List<Token>(){ new Token(){tknType=TokenType.DOCS,info="info"},
                                              new Token(){
                                                  tknType=TokenType.OUTPUT,
                                                  val=CompilerMessage.COMPILER_HELP
                                              }
                                          }
                    },
                    { "???", new List<Token>(){ new Token(){tknType=TokenType.DOCS,info="info"},
                                                new Token(){
                                                  tknType=TokenType.OUTPUT,
                                                  val=CompilerMessage.COMPILER_HELP
                                                }
                                            }
                    },
                    { "?Rank", new List<Token>(){
                                                 new Token(){tknType=TokenType.DOCS,info=RankInfo.Info()},
                                                 new Token(){
                                                  tknType=TokenType.OUTPUT,
                                                  val=RankInfo.Info()
                                                 }
                                            }
                    },
                    { "?Transpose", new List<Token>(){
                                                 new Token(){tknType=TokenType.DOCS,info=TransposeInfo.Info()},
                                                 new Token(){
                                                  tknType=TokenType.OUTPUT,
                                                  val=Transpose.Details("Transpose") +
                                                                "\nKomut: " + "Transpose" + "\n" +TransposeInfo.Info()
                                                 }
                                            }
                    }
                }
            }
        };

        private void Tokenize_And_Evaluate_Command(Dictionary<string, List<Token>> cmds, Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            // Loop through expressions
            foreach (string cmd in cmds.Keys)
            {
                int tknind = 0;

                // Create a command
                Command command = new Command(cmd);

                // Tokenize 
                command.Tokens = FrontService.Tokenize(command.TermsToEvaluate[0]);

                // Check tokens
                foreach (Token tkn in command.Tokens)
                {
                    // Check token type
                    Assert.AreEqual(cmds[cmd][tknind].tknType, tkn.tknType,
                        "\n" + cmd + "\nToken tipleri uyuşmadı! \nBeklenen:" + cmds[cmd][tknind].tknType.ToString() + " \nAlınan:" + tkn.tknType.ToString());

                    // Check matrix names
                    if (tkn.tknType == TokenType.MATRIS)
                    {
                        Assert.AreEqual(cmds[cmd][tknind].name, tkn.name,
                        "\n" + cmd + "\nToken isimleri uyuşmadı! \nBeklenen:" + cmds[cmd][tknind].name + " \nAlınan:" + tkn.name);
                    }

                    // Check symbol
                    else if (tkn.tknType == TokenType.OPERATOR)
                    {
                        Assert.AreEqual(cmds[cmd][tknind].symbol, tkn.symbol,
                        "\n" + cmd + "\nToken sembolleri uyuşmadı! \nBeklenen:" + cmds[cmd][tknind].symbol + " \nAlınan:" + tkn.symbol);
                    }

                    // Check docs
                    else if (tkn.tknType == TokenType.DOCS)
                    {
                        Assert.AreEqual(cmds[cmd][tknind].info, tkn.info,
                        "\n" + cmd + "\nToken info uyuşmadı! \nBeklenen:" + cmds[cmd][tknind].info + " \nAlınan:" + tkn.info);
                    }

                    // Check token val
                    else
                    {
                        Assert.AreEqual(cmds[cmd][tknind].val, tkn.val,
                        "\n" + cmd + "\nToken değerleri uyuşmadı! \nBeklenen:" + cmds[cmd][tknind].val.ToString() + " \nAlınan:" + tkn.val.ToString());
                    }

                    tknind++;
                }

                // If last token is output , evaluate command and check output
                if (cmds[cmd][^1].tknType == TokenType.OUTPUT)
                {
                    command.Tokens = FrontService.ShuntingYardAlg(command.Tokens);  // Order tokens

                    // Check evaluating state
                    Assert.AreEqual(FrontService.EvaluateCommand(command, matdict, new List<Command>()), CommandState.SUCCESS,
                        "\n" + cmd + "\nÇözümleme başarısız! \n" + command.CommandSummary());

                    // Check output
                    Assert.AreEqual(command.Output.ToString(), cmds[cmd][^1].val.ToString(),
                        "\n" + cmd + "\nÇıktılar uyuşmadı! \nBeklenen:" + cmds[cmd][^1].val.ToString() + " \nÇıktı:" + command.Output.ToString());
                }
            }
        }

        [TestMethod]
        public void Basit_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Basit_Aritmetik"],
                new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Unary_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Unary_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Extreme_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Extreme_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Priority_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Priority_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Associative_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Associative_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Matris_Basit_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Matris_Basit_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
                   {
                        {"A", A}
                   }
            );
        }

        [TestMethod]
        public void Matris_Custom_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Matris_Custom_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
                   {
                        {"A", A}
                   }
            );
        }

        [TestMethod]
        public void Matris_Special_Service()
        {
            Tokenize_And_Evaluate_Command(CMDS["Matris_Special_Service"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Matris_Combined_Aritmetik()
        {
            Tokenize_And_Evaluate_Command(CMDS["Matris_Combined_Aritmetik"],
                   new Dictionary<string, MatrisBase<dynamic>>()
            );
        }

        [TestMethod]
        public void Docs()
        {
            Tokenize_And_Evaluate_Command(CMDS["Docs"],
                   new Dictionary<string, MatrisBase<dynamic>>()
                   {
                       {"Transpose", Transpose }
                   }
            );
        }
    }
}
