/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.states;
using System.Reflection;
using System.Text;

namespace org.antlr.v4.test.tool;


//@SuppressWarnings("unused")
[TestClass]
public class TestPerformance
{
    public static readonly String NewLine = Environment.NewLine;
    /**
     * Parse all java files under this package within the JDK_SOURCE_ROOT
     * (environment variable or property defined on the Java command line).
     */
    private static readonly String TOP_PACKAGE = "java.lang";
    /**
     * {@code true} to load java files from sub-packages of
     * {@link #TOP_PACKAGE}.
     */
    private static readonly bool RECURSIVE = true;
    /**
	 * {@code true} to read all source files from disk into memory before
	 * starting the parse. The default value is {@code true} to help prevent
	 * drive speed from affecting the performance results. This value may be set
	 * to {@code false} to support parsing large input sets which would not
	 * otherwise fit into memory.
	 */
    private static readonly bool PRELOAD_SOURCES = true;
    /**
	 * The encoding to use when reading source files.
	 */
    private static readonly Encoding ENCODING = Encoding.UTF8;
    /**
	 * The maximum number of files to parse in a single iteration.
	 */
    private static readonly int MAX_FILES_PER_PARSE_ITERATION = int.MaxValue;

    /**
	 * {@code true} to call {@link Collections#shuffle} on the list of input
	 * files before the first parse iteration.
	 */
    private static readonly bool SHUFFLE_FILES_AT_START = false;
    /**
	 * {@code true} to call {@link Collections#shuffle} before each parse
	 * iteration <em>after</em> the first.
	 */
    private static readonly bool SHUFFLE_FILES_AFTER_ITERATIONS = false;
    /**
	 * The instance of {@link Random} passed when calling
	 * {@link Collections#shuffle}.
	 */
    private static readonly Random RANDOM = new Random();

    /**
     * {@code true} to use the Java grammar with expressions in the v4
     * left-recursive syntax (JavaLR.g4). {@code false} to use the standard
     * grammar (Java.g4). In either case, the grammar is renamed in the
     * temporary directory to Java.g4 before compiling.
     */
    private static readonly bool USE_LR_GRAMMAR = true;
    /**
     * {@code true} to specify the {@code -Xforce-atn} option when generating
     * the grammar, forcing all decisions in {@code JavaParser} to be handled by
     * {@link ParserATNSimulator#adaptivePredict}.
     */
    private static readonly bool FORCE_ATN = false;
    /**
     * {@code true} to specify the {@code -atn} option when generating the
     * grammar. This will cause ANTLR to export the ATN for each decision as a
     * DOT (GraphViz) file.
     */
    private static readonly bool EXPORT_ATN_GRAPHS = true;
    /**
	 * {@code true} to specify the {@code -XdbgST} option when generating the
	 * grammar.
	 */
    private static readonly bool DEBUG_TEMPLATES = false;
    /**
	 * {@code true} to specify the {@code -XdbgSTWait} option when generating the
	 * grammar.
	 */
    private static readonly bool DEBUG_TEMPLATES_WAIT = DEBUG_TEMPLATES;
    /**
     * {@code true} to delete temporary (generated and compiled) files when the
     * test completes.
     */
    private static readonly bool DELETE_TEMP_FILES = true;
    /**
	 * {@code true} to use a {@link ParserInterpreter} for parsing instead of
	 * generated parser.
	 */
    private static readonly bool USE_PARSER_INTERPRETER = false;

    /**
	 * {@code true} to call {@link System#gc} and then wait for 5 seconds at the
	 * end of the test to make it easier for a profiler to grab a heap dump at
	 * the end of the test run.
	 */
    private static readonly bool PAUSE_FOR_HEAP_DUMP = false;

    /**
     * Parse each file with {@code JavaParser.compilationUnit}.
     */
    private static readonly bool RUN_PARSER = true;
    /**
     * {@code true} to use {@link BailErrorStrategy}, {@code false} to use
     * {@link DefaultErrorStrategy}.
     */
    private static readonly bool BAIL_ON_ERROR = false;
    /**
	 * {@code true} to compute a checksum for verifying consistency across
	 * optimizations and multiple passes.
	 */
    private static readonly bool COMPUTE_CHECKSUM = true;
    /**
     * This value is passed to {@link Parser#setBuildParseTree}.
     */
    private static readonly bool BUILD_PARSE_TREES = false;
    /**
     * Use
     * {@link ParseTreeWalker#DEFAULT}{@code .}{@link ParseTreeWalker#walk walk}
     * with the {@code JavaParserBaseListener} to show parse tree walking
     * overhead. If {@link #BUILD_PARSE_TREES} is {@code false}, the listener
     * will instead be called during the parsing process via
     * {@link Parser#addParseListener}.
     */
    private static readonly bool BLANK_LISTENER = false;

    /**
	 * Shows the number of {@link DFAState} and {@link ATNConfig} instances in
	 * the DFA cache at the end of each pass. If {@link #REUSE_LEXER_DFA} and/or
	 * {@link #REUSE_PARSER_DFA} are false, the corresponding instance numbers
	 * will only apply to one file (the last file if {@link #NUMBER_OF_THREADS}
	 * is 0, otherwise the last file which was parsed on the first thread).
	 */
    private static readonly bool SHOW_DFA_STATE_STATS = true;
    /**
	 * If {@code true}, the DFA state statistics report includes a breakdown of
	 * the number of DFA states contained in each decision (with rule names).
	 */
    private static readonly bool DETAILED_DFA_STATE_STATS = true;

    /**
	 * Specify the {@link PredictionMode} used by the
	 * {@link ParserATNSimulator}. If {@link #TWO_STAGE_PARSING} is
	 * {@code true}, this value only applies to the second stage, as the first
	 * stage will always use {@link PredictionMode#SLL}.
	 */
    private static readonly PredictionMode PREDICTION_MODE = PredictionMode.LL;

    private static readonly bool TWO_STAGE_PARSING = true;

    private static readonly bool SHOW_CONFIG_STATS = false;

    /**
	 * If {@code true}, detailed statistics for the number of DFA edges were
	 * taken while parsing each file, as well as the number of DFA edges which
	 * required on-the-fly computation.
	 */
    private static readonly bool COMPUTE_TRANSITION_STATS = false;
    private static readonly bool SHOW_TRANSITION_STATS_PER_FILE = false;
    /**
	 * If {@code true}, the transition statistics will be adjusted to a running
	 * total before reporting the readonly results.
	 */
    private static readonly bool TRANSITION_RUNNING_AVERAGE = false;
    /**
	 * If {@code true}, transition statistics will be weighted according to the
	 * total number of transitions taken during the parsing of each file.
	 */
    private static readonly bool TRANSITION_WEIGHTED_AVERAGE = false;

    /**
	 * If {@code true}, after each pass a summary of the time required to parse
	 * each file will be printed.
	 */
    private static readonly bool COMPUTE_TIMING_STATS = false;
    /**
	 * If {@code true}, the timing statistics for {@link #COMPUTE_TIMING_STATS}
	 * will be cumulative (i.e. the time reported for the <em>n</em>th file will
	 * be the total time required to parse the first <em>n</em> files).
	 */
    private static readonly bool TIMING_CUMULATIVE = false;
    /**
	 * If {@code true}, the timing statistics will include the parser only. This
	 * flag allows for targeted measurements, and helps eliminate variance when
	 * {@link #PRELOAD_SOURCES} is {@code false}.
	 * <p/>
	 * This flag has no impact when {@link #RUN_PARSER} is {@code false}.
	 */
    private static readonly bool TIME_PARSE_ONLY = false;

    /**
	 * When {@code true}, messages will be printed to {@link System#err} when
	 * the first stage (SLL) parsing resulted in a syntax error. This option is
	 * ignored when {@link #TWO_STAGE_PARSING} is {@code false}.
	 */
    private static readonly bool REPORT_SECOND_STAGE_RETRY = true;
    private static readonly bool REPORT_SYNTAX_ERRORS = true;
    private static readonly bool REPORT_AMBIGUITIES = false;
    private static readonly bool REPORT_FULL_CONTEXT = false;
    private static readonly bool REPORT_CONTEXT_SENSITIVITY = REPORT_FULL_CONTEXT;

    /**
     * If {@code true}, a single {@code JavaLexer} will be used, and
     * {@link Lexer#setInputStream} will be called to initialize it for each
     * source file. Otherwise, a new instance will be created for each file.
     */
    private static readonly bool REUSE_LEXER = false;
    /**
	 * If {@code true}, a single DFA will be used for lexing which is shared
	 * across all threads and files. Otherwise, each file will be lexed with its
	 * own DFA which is accomplished by creating one ATN instance per thread and
	 * clearing its DFA cache before lexing each file.
	 */
    private static readonly bool REUSE_LEXER_DFA = true;
    /**
     * If {@code true}, a single {@code JavaParser} will be used, and
     * {@link Parser#setInputStream} will be called to initialize it for each
     * source file. Otherwise, a new instance will be created for each file.
     */
    private static readonly bool REUSE_PARSER = false;
    /**
	 * If {@code true}, a single DFA will be used for parsing which is shared
	 * across all threads and files. Otherwise, each file will be parsed with
	 * its own DFA which is accomplished by creating one ATN instance per thread
	 * and clearing its DFA cache before parsing each file.
	 */
    private static readonly bool REUSE_PARSER_DFA = true;
    /**
     * If {@code true}, the shared lexer and parser are reset after each pass.
     * If {@code false}, all passes after the first will be fully "warmed up",
     * which makes them faster and can compare them to the first warm-up pass,
     * but it will not distinguish bytecode load/JIT time from warm-up time
     * during the first pass.
     */
    private static readonly bool CLEAR_DFA = false;
    /**
     * Total number of passes to make over the source.
     */
    private static readonly int PASSES = 4;

    /**
	 * This option controls the granularity of multi-threaded parse operations.
	 * If {@code true}, the parsing operation will be parallelized across files;
	 * otherwise the parsing will be parallelized across multiple iterations.
	 */
    private static readonly bool FILE_GRANULARITY = true;

    /**
	 * Number of parser threads to use.
	 */
    private static readonly int NUMBER_OF_THREADS = 1;

    private static readonly Lexer[] sharedLexers = new Lexer[NUMBER_OF_THREADS];

    private static readonly Parser[] sharedParsers = new Parser[NUMBER_OF_THREADS];

    private static readonly ParseTreeListener[] sharedListeners = new ParseTreeListener[NUMBER_OF_THREADS];

    private static readonly long[][] totalTransitionsPerFile;
    private static readonly long[][] computedTransitionsPerFile;

    private static readonly long[][][] decisionInvocationsPerFile;
    private static readonly long[][][] fullContextFallbackPerFile;
    private static readonly long[][][] nonSllPerFile;
    private static readonly long[][][] totalTransitionsPerDecisionPerFile;
    private static readonly long[][][] computedTransitionsPerDecisionPerFile;
    private static readonly long[][][] fullContextTransitionsPerDecisionPerFile;
    private static readonly long[][] timePerFile;
    private static readonly int[][] tokensPerFile;
    static TestPerformance()
    {
        if (COMPUTE_TRANSITION_STATS)
        {
            totalTransitionsPerFile = new long[PASSES][];
            computedTransitionsPerFile = new long[PASSES][];
        }
        else
        {
            totalTransitionsPerFile = null;
            computedTransitionsPerFile = null;
        }
        if (COMPUTE_TRANSITION_STATS && DETAILED_DFA_STATE_STATS)
        {
            decisionInvocationsPerFile = new long[PASSES][][];
            fullContextFallbackPerFile = new long[PASSES][][];
            nonSllPerFile = new long[PASSES][][];
            totalTransitionsPerDecisionPerFile = new long[PASSES][][];
            computedTransitionsPerDecisionPerFile = new long[PASSES][][];
            fullContextTransitionsPerDecisionPerFile = new long[PASSES][][];
        }
        else
        {
            decisionInvocationsPerFile = null;
            fullContextFallbackPerFile = null;
            nonSllPerFile = null;
            totalTransitionsPerDecisionPerFile = null;
            computedTransitionsPerDecisionPerFile = null;
            fullContextTransitionsPerDecisionPerFile = null;
        }
        if (COMPUTE_TIMING_STATS)
        {
            timePerFile = new long[PASSES][];
            tokensPerFile = new int[PASSES][];
        }
        else
        {
            timePerFile = null;
            tokensPerFile = null;
        }
    }
    //public class R0new : Runnable
    //{
    //    //@Override
    //    public void run()
    //    {
    //        try
    //        {
    //            //parse1(0, factory, sources, SHUFFLE_FILES_AT_START);
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.getLogger(TestPerformance.getName()).log(Level.SEVERE, null, ex);
    //        }
    //    }
    //}

    private static object getName()
    {
        throw new NotImplementedException();
    }
    //public class R1
    //{
    //    //@Override
    //    public void run()
    //    {
    //        if (CLEAR_DFA)
    //        {
    //            int index = FILE_GRANULARITY ? 0 : ((NumberedThread)Thread.CurrentThread).getThreadNumber();
    //            if (sharedLexers.Length > 0 && sharedLexers[index] != null)
    //            {
    //                ATN atn = sharedLexers[index].getATN();
    //                for (int j = 0; j < sharedLexers[index].getInterpreter().decisionToDFA.Length; j++)
    //                {
    //                    sharedLexers[index].getInterpreter().decisionToDFA[j] = new DFA(atn.getDecisionState(j), j);
    //                }
    //            }

    //            if (sharedParsers.Length > 0 && sharedParsers[index] != null)
    //            {
    //                ATN atn = sharedParsers[index].getATN();
    //                for (int j = 0; j < sharedParsers[index].getInterpreter().decisionToDFA.Length; j++)
    //                {
    //                    sharedParsers[index].getInterpreter().decisionToDFA[j] = new DFA(atn.getDecisionState(j), j);
    //                }
    //            }

    //            if (FILE_GRANULARITY)
    //            {
    //                Arrays.Fill(sharedLexers, null);
    //                Arrays.Fill(sharedParsers, null);
    //            }
    //        }

    //        try
    //        {
    //            parse2(currentPass, factory, sources, SHUFFLE_FILES_AFTER_ITERATIONS);
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.getLogger(TestPerformance.getName()).log(Level.SEVERE, null, ex);
    //        }
    //    }
    //}
    //private readonly AtomicIntegerArray tokenCount = new AtomicIntegerArray(PASSES);
    //[TestMethod]
    ////@Disabled
    //public void compileJdk()
    //{
    //    String jdkSourceRoot = getSourceRoot("JDK");
    //    Assert.IsTrue(jdkSourceRoot != null && jdkSourceRoot.Length > 0,
    //            "The JDK_SOURCE_ROOT environment variable must be set for performance testing.");

    //    JavaCompiledState javaCompiledState = compileJavaParser(USE_LR_GRAMMAR);
    //    String lexerName = USE_LR_GRAMMAR ? "JavaLRLexer" : "JavaLexer";
    //    String parserName = USE_LR_GRAMMAR ? "JavaLRParser" : "JavaParser";
    //    String listenerName = USE_LR_GRAMMAR ? "JavaLRBaseListener" : "JavaBaseListener";
    //    String entryPoint = "compilationUnit";
    //    ParserFactory factory = getParserFactory(javaCompiledState, listenerName, entryPoint);

    //    if (TOP_PACKAGE.Length > 0)
    //    {
    //        jdkSourceRoot = jdkSourceRoot + '/' + TOP_PACKAGE.Replace('.', '/');
    //    }

    //    string directory = (jdkSourceRoot);
    //    Assert.IsTrue(directory.isDirectory());

    //    FilenameFilter filesFilter = FilenameFilters.extension(".java", false);
    //    FilenameFilter directoriesFilter = FilenameFilters.ALL_FILES;
    //    List<InputDescriptor> sources = loadSources(directory, filesFilter, directoriesFilter, RECURSIVE);

    //    for (int i = 0; i < PASSES; i++)
    //    {
    //        if (COMPUTE_TRANSITION_STATS)
    //        {
    //            totalTransitionsPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)];
    //            computedTransitionsPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)];

    //            if (DETAILED_DFA_STATE_STATS)
    //            {
    //                decisionInvocationsPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //                fullContextFallbackPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //                nonSllPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //                totalTransitionsPerDecisionPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //                computedTransitionsPerDecisionPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //                fullContextTransitionsPerDecisionPerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)][];
    //            }
    //        }

    //        if (COMPUTE_TIMING_STATS)
    //        {
    //            timePerFile[i] = new long[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)];
    //            tokensPerFile[i] = new int[Math.Min(sources.Count, MAX_FILES_PER_PARSE_ITERATION)];
    //        }
    //    }

    //    Console.Out.Write($"Located {sources.Count} source files.%n");
    //    Console.Out.Write(getOptionsDescription(TOP_PACKAGE));

    //    ExecutorService executorService = Executors.newFixedThreadPool(FILE_GRANULARITY ? 1 : NUMBER_OF_THREADS, new NumberedThreadFactory());

    //    List<Future> passResults = new();
    //    passResults.Add(executorService.submit(new R0()));
    //    for (int i = 0; i < PASSES - 1; i++)
    //    {
    //        int currentPass = i + 1;
    //        passResults.Add(executorService.submit(new R1()));
    //    }

    //    foreach (Future passResult in passResults)
    //    {
    //        passResult.get();
    //    }

    //    executorService.shutdown();
    //    executorService.awaitTermination(Long.MAX_VALUE, TimeUnit.NANOSECONDS);

    //    if (COMPUTE_TRANSITION_STATS && SHOW_TRANSITION_STATS_PER_FILE)
    //    {
    //        computeTransitionStatistics();
    //    }

    //    if (COMPUTE_TIMING_STATS)
    //    {
    //        computeTimingStatistics();
    //    }

    //    sources.Clear();
    //    if (PAUSE_FOR_HEAP_DUMP)
    //    {
    //        GC.Collect();
    //        Console.Out.WriteLine("Pausing before application exit.");
    //        try
    //        {
    //            Thread.Sleep(4000);
    //        }
    //        catch (InterruptedException ex)
    //        {
    //            Logger.getLogger(TestPerformance.getName()).log(Level.SEVERE, null, ex);
    //        }
    //    }
    //}

    /**
	 * Compute and print ATN/DFA transition statistics.
	 */
  //  private void computeTransitionStatistics()
  //  {
  //      if (TRANSITION_RUNNING_AVERAGE)
  //      {
  //          for (int i = 0; i < PASSES; i++)
  //          {
  //              long[] data = computedTransitionsPerFile[i];
  //              for (int j = 0; j < data.Length - 1; j++)
  //              {
  //                  data[j + 1] += data[j];
  //              }

  //              data = totalTransitionsPerFile[i];
  //              for (int j = 0; j < data.Length - 1; j++)
  //              {
  //                  data[j + 1] += data[j];
  //              }
  //          }
  //      }

  //      long[] sumNum = new long[totalTransitionsPerFile[0].Length];
  //      long[] sumDen = new long[totalTransitionsPerFile[0].Length];
  //      double[] sumNormalized = new double[totalTransitionsPerFile[0].Length];
  //      for (int i = 0; i < PASSES; i++)
  //      {
  //          long[] num = computedTransitionsPerFile[i];
  //          long[] den = totalTransitionsPerFile[i];
  //          for (int j = 0; j < den.Length; j++)
  //          {
  //              sumNum[j] += num[j];
  //              sumDen[j] += den[j];
  //              if (den[j] > 0)
  //              {
  //                  sumNormalized[j] += (double)num[j] / (double)den[j];
  //              }
  //          }
  //      }

  //      double[] weightedAverage = new double[totalTransitionsPerFile[0].Length];
  //      double[] average = new double[totalTransitionsPerFile[0].Length];
  //      for (int i = 0; i < average.Length; i++)
  //      {
  //          if (sumDen[i] > 0)
  //          {
  //              weightedAverage[i] = (double)sumNum[i] / (double)sumDen[i];
  //          }
  //          else
  //          {
  //              weightedAverage[i] = 0;
  //          }

  //          average[i] = sumNormalized[i] / PASSES;
  //      }

  //      double[] low95 = new double[totalTransitionsPerFile[0].Length];
  //      double[] high95 = new double[totalTransitionsPerFile[0].Length];
  //      double[] low67 = new double[totalTransitionsPerFile[0].Length];
  //      double[] high67 = new double[totalTransitionsPerFile[0].Length];
  //      double[] stddev = new double[totalTransitionsPerFile[0].Length];
  //      for (int i = 0; i < stddev.Length; i++)
  //      {
  //          double[] points = new double[PASSES];
  //          for (int j = 0; j < PASSES; j++)
  //          {
  //              long totalTransitions = totalTransitionsPerFile[j][i];
  //              if (totalTransitions > 0)
  //              {
  //                  points[j] = ((double)computedTransitionsPerFile[j][i] / (double)totalTransitionsPerFile[j][i]);
  //              }
  //              else
  //              {
  //                  points[j] = 0;
  //              }
  //          }

  //          Array.Sort(points);

  //          double averageValue = TRANSITION_WEIGHTED_AVERAGE ? weightedAverage[i] : average[i];
  //          double value = 0;
  //          for (int j = 0; j < PASSES; j++)
  //          {
  //              double diff = points[j] - averageValue;
  //              value += diff * diff;
  //          }

  //          int ignoreCount95 = (int)Math.Round(PASSES * (1 - 0.95) / 2.0);
  //          int ignoreCount67 = (int)Math.Round(PASSES * (1 - 0.667) / 2.0);
  //          low95[i] = points[ignoreCount95];
  //          high95[i] = points[points.Length - 1 - ignoreCount95];
  //          low67[i] = points[ignoreCount67];
  //          high67[i] = points[points.Length - 1 - ignoreCount67];
  //          stddev[i] = Math.Sqrt(value / PASSES);
  //      }

  //      Console.Out.WriteLine("File\tAverage\tStd. Dev.\t95%% Low\t95%% High\t66.7%% Low\t66.7%% High%n");
  //      for (int i = 0; i < stddev.Length; i++)
  //      {
  //          double averageValue = TRANSITION_WEIGHTED_AVERAGE ? weightedAverage[i] : average[i];
  //          Console.Out.format("%d\t%e\t%e\t%e\t%e\t%e\t%e%n", i + 1, averageValue, stddev[i], averageValue - low95[i], high95[i] - averageValue, averageValue - low67[i], high67[i] - averageValue);
  //      }
  //  }
  //  /**
	 //* Compute and print timing statistics.
	 //*/
  //  private void computeTimingStatistics()
  //  {
  //      if (TIMING_CUMULATIVE)
  //      {
  //          for (int i = 0; i < PASSES; i++)
  //          {
  //              long[] data = timePerFile[i];
  //              for (int j = 0; j < data.Length - 1; j++)
  //              {
  //                  data[j + 1] += data[j];
  //              }

  //              int[] data2 = tokensPerFile[i];
  //              for (int j = 0; j < data2.Length - 1; j++)
  //              {
  //                  data2[j + 1] += data2[j];
  //              }
  //          }
  //      }

  //      int fileCount = timePerFile[0].Length;
  //      double[] sum = new double[fileCount];
  //      for (int i = 0; i < PASSES; i++)
  //      {
  //          long[] data = timePerFile[i];
  //          int[] tokenData = tokensPerFile[i];
  //          for (int j = 0; j < data.Length; j++)
  //          {
  //              sum[j] += (double)data[j] / (double)tokenData[j];
  //          }
  //      }

  //      double[] average = new double[fileCount];
  //      for (int i = 0; i < average.Length; i++)
  //      {
  //          average[i] = sum[i] / PASSES;
  //      }

  //      double[] low95 = new double[fileCount];
  //      double[] high95 = new double[fileCount];
  //      double[] low67 = new double[fileCount];
  //      double[] high67 = new double[fileCount];
  //      double[] stddev = new double[fileCount];
  //      for (int i = 0; i < stddev.Length; i++)
  //      {
  //          double[] points = new double[PASSES];
  //          for (int j = 0; j < PASSES; j++)
  //          {
  //              points[j] = (double)timePerFile[j][i] / (double)tokensPerFile[j][i];
  //          }

  //          Array.Sort(points);

  //          double averageValue = average[i];
  //          double value = 0;
  //          for (int j = 0; j < PASSES; j++)
  //          {
  //              double diff = points[j] - averageValue;
  //              value += diff * diff;
  //          }

  //          int ignoreCount95 = (int)Math.Round(PASSES * (1 - 0.95) / 2.0);
  //          int ignoreCount67 = (int)Math.Round(PASSES * (1 - 0.667) / 2.0);
  //          low95[i] = points[ignoreCount95];
  //          high95[i] = points[points.Length - 1 - ignoreCount95];
  //          low67[i] = points[ignoreCount67];
  //          high67[i] = points[points.Length - 1 - ignoreCount67];
  //          stddev[i] = Math.Sqrt(value / PASSES);
  //      }

  //      Console.Out.WriteLine("File\tAverage\tStd. Dev.\t95%% Low\t95%% High\t66.7%% Low\t66.7%% High%n");
  //      for (int i = 0; i < stddev.Length; i++)
  //      {
  //          double averageValue = average[i];
  //          Console.Out.WriteLine("%d\t%e\t%e\t%e\t%e\t%e\t%e%n", i + 1, averageValue, stddev[i], averageValue - low95[i], high95[i] - averageValue, averageValue - low67[i], high67[i] - averageValue);
  //      }
  //  }

  //  private String getSourceRoot(String prefix)
  //  {
  //      String sourceRoot =Environment.GetEnvironmentVariable(prefix + "_SOURCE_ROOT");
  //      //if (sourceRoot == null)
  //      //{
  //      //    sourceRoot = System.getProperty(prefix + "_SOURCE_ROOT");
  //      //}

  //      return sourceRoot;
  //  }

  //  public static String getOptionsDescription(String topPackage)
  //  {
  //      StringBuilder builder = new StringBuilder();
  //      builder.Append("Input=");
  //      if (topPackage.Length == 0)
  //      {
  //          builder.Append("*");
  //      }
  //      else
  //      {
  //          builder.Append(topPackage).Append(".*");
  //      }

  //      builder.Append(", Grammar=").Append(USE_LR_GRAMMAR ? "LR" : "Standard");
  //      builder.Append(", ForceAtn=").Append(FORCE_ATN);

  //      builder.Append(NewLine);

  //      builder.Append("Op=Lex").Append(RUN_PARSER ? "+Parse" : " only");
  //      builder.Append(", Strategy=").Append(BAIL_ON_ERROR ? nameof(BailErrorStrategy) : nameof(DefaultErrorStrategy));
  //      builder.Append(", BuildParseTree=").Append(BUILD_PARSE_TREES);
  //      builder.Append(", WalkBlankListener=").Append(BLANK_LISTENER);

  //      builder.Append(NewLine);

  //      builder.Append("Lexer=").Append(REUSE_LEXER ? "setInputStream" : "newInstance");
  //      builder.Append(", Parser=").Append(REUSE_PARSER ? "setInputStream" : "newInstance");
  //      builder.Append(", AfterPass=").Append(CLEAR_DFA ? "newInstance" : "setInputStream");

  //      builder.Append(NewLine);

  //      return builder.ToString();
  //  }

    /**
     *  This method is separate from {@link #parse2} so the first pass can be distinguished when analyzing
     *  profiler results.
     */
    //protected void parse1(int currentPass, ParserFactory factory, ICollection<InputDescriptor> sources, bool shuffleSources)
    //{
    //    if (FILE_GRANULARITY)
    //    {
    //        GC.Collect();
    //    }

    //    parseSources(currentPass, factory, sources, shuffleSources);
    //}

    /**
     *  This method is separate from {@link #parse1} so the first pass can be distinguished when analyzing
     *  profiler results.
     */
    //protected void parse2(int currentPass, ParserFactory factory, ICollection<InputDescriptor> sources, bool shuffleSources)
    //{
    //    if (FILE_GRANULARITY)
    //    {
    //        GC.Collect();
    //    }

    //    parseSources(currentPass, factory, sources, shuffleSources);
    //}

    protected List<InputDescriptor> loadSources(string directory, string filesFilter, string directoriesFilter, bool recursive)
    {
        List<InputDescriptor> result = new();
        loadSources(directory, filesFilter, directoriesFilter, recursive, result);
        return result;
    }

    protected void loadSources(string directory, string filesFilter, string directoriesFilter, bool recursive, ICollection<InputDescriptor> result)
    {
        //assert directory.isDirectory();

        var sources = new DirectoryInfo(directory).GetFiles(filesFilter);
        foreach (var file in sources)
        {
            if ((file.Attributes&FileAttributes.Normal)!= FileAttributes.Normal )
            {
                continue;
            }

            result.Add(new InputDescriptor(file.FullName));
        }

        if (recursive)
        {
            var children = new DirectoryInfo(directory).GetFiles(directoriesFilter, SearchOption.AllDirectories);
            foreach (var child in children)
            {
                if ((child.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    loadSources(child.FullName, filesFilter, directoriesFilter, true, result);
                }
            }
        }
    }

    int configOutputSize = 0;

//    protected void parseSources(int currentPass, ParserFactory factory, ICollection<InputDescriptor> sources, bool shuffleSources)
//    {
//        if (shuffleSources)
//        {
//            List<InputDescriptor> sourcesList = new(sources);
//            lock (RANDOM)
//            {
//                Collections.shuffle(sourcesList, RANDOM);
//            }

//            sources = sourcesList;
//        }

//        long startTime = DateTime.Now.Nanosecond;
//        tokenCount.set(currentPass, 0);
//        int inputSize = 0;
//        int inputCount = 0;

//        ICollection<Future<FileParseResult>> results = new();
//        ExecutorService executorService;
//        if (FILE_GRANULARITY)
//        {
//            executorService = Executors.newFixedThreadPool(FILE_GRANULARITY ? NUMBER_OF_THREADS : 1, new NumberedThreadFactory());
//        }
//        else
//        {
//            executorService = Executors.newSingleThreadExecutor(new FixedThreadNumberFactory(((NumberedThread)Thread.currentThread()).getThreadNumber()));
//        }

//        foreach (InputDescriptor inputDescriptor in sources)
//        {
//            if (inputCount >= MAX_FILES_PER_PARSE_ITERATION)
//            {
//                break;
//            }

//            CharStream input = inputDescriptor.getInputStream();
//            input.seek(0);
//            inputSize += input.size();
//            inputCount++;
//            //TODO:
//#if false
//            Future<FileParseResult> futureChecksum = executorService.submit(new Callable<FileParseResult>()
//            {
//                //@Override
//                public FileParseResult call()
//            {
//                // this incurred a great deal of overhead and was causing significant variations in performance results.
//                //Console.Out.format("Parsing file %s\n", input.getSourceName());
//                try
//                {
//                    return factory.parseFile(input, currentPass, ((NumberedThread)Thread.currentThread()).getThreadNumber());
//                }
//                catch (IllegalStateException ex)
//                {
//                    ex.printStackTrace(System.err);
//                }
//                catch (Throwable t)
//                {
//                    t.printStackTrace(System.err);
//                }

//                return null;
//            }
//        });
//#endif
//            results.Add(futureChecksum);
//        }

//        MurmurHashChecksum checksum = new MurmurHashChecksum();
//        int currentIndex = -1;
//        foreach (Future<FileParseResult> future in results)
//        {
//            currentIndex++;
//            int fileChecksum = 0;
//            try
//            {
//                FileParseResult fileResult = future.get();
//                if (COMPUTE_TRANSITION_STATS)
//                {
//                    totalTransitionsPerFile[currentPass][currentIndex] = sum(fileResult.parserTotalTransitions);
//                    computedTransitionsPerFile[currentPass][currentIndex] = sum(fileResult.parserComputedTransitions);

//                    if (DETAILED_DFA_STATE_STATS)
//                    {
//                        decisionInvocationsPerFile[currentPass][currentIndex] = fileResult.decisionInvocations;
//                        fullContextFallbackPerFile[currentPass][currentIndex] = fileResult.fullContextFallback;
//                        nonSllPerFile[currentPass][currentIndex] = fileResult.nonSll;
//                        totalTransitionsPerDecisionPerFile[currentPass][currentIndex] = fileResult.parserTotalTransitions;
//                        computedTransitionsPerDecisionPerFile[currentPass][currentIndex] = fileResult.parserComputedTransitions;
//                        fullContextTransitionsPerDecisionPerFile[currentPass][currentIndex] = fileResult.parserFullContextTransitions;
//                    }
//                }

//                if (COMPUTE_TIMING_STATS)
//                {
//                    timePerFile[currentPass][currentIndex] = fileResult.endTime - fileResult.startTime;
//                    tokensPerFile[currentPass][currentIndex] = fileResult.tokenCount;
//                }

//                fileChecksum = fileResult.checksum;
//            }
//            catch (Exception ex)
//            {
//                Logger.getLogger(TestPerformance.getName()).log(Level.SEVERE, null, ex);
//            }

//            if (COMPUTE_CHECKSUM)
//            {
//                updateChecksum(checksum, fileChecksum);
//            }
//        }

//        executorService.shutdown();
//        executorService.awaitTermination(long.MaxValue, TimeUnit.NANOSECONDS);

//        Console.Out.format("%d. Total parse time for %d files (%d KB, %d tokens%s): %.0fms%n",
//                          currentPass + 1,
//                          inputCount,
//                          inputSize / 1024,
//                          tokenCount.get(currentPass),
//                          COMPUTE_CHECKSUM ? String.format(", checksum 0x%8X", checksum.getValue()) : "",
//                          (double)(DateTime.Now.Nanosecond - startTime) / 1000000.0);

//        if (sharedLexers.Length > 0)
//        {
//            int index = FILE_GRANULARITY ? 0 : ((NumberedThread)Thread.currentThread()).getThreadNumber();
//            Lexer lexer = sharedLexers[index];
//            LexerATNSimulator lexerInterpreter = lexer.getInterpreter();
//            DFA[] modeToDFA = lexerInterpreter.decisionToDFA;
//            if (SHOW_DFA_STATE_STATS)
//            {
//                int states = 0;
//                int configs = 0;
//                HashSet<ATNConfig> uniqueConfigs = new HashSet<ATNConfig>();

//                for (int i = 0; i < modeToDFA.Length; i++)
//                {
//                    DFA dfa = modeToDFA[i];
//                    if (dfa == null)
//                    {
//                        continue;
//                    }

//                    states += dfa.states.Count;
//                    foreach (DFAState state in dfa.states.Values)
//                    {
//                        configs += state.configs.Count;
//                        uniqueConfigs.UnionWith(state.configs);
//                    }
//                }

//                Console.Out.format("There are %d lexer DFAState instances, %d configs (%d unique).%n", states, configs, uniqueConfigs.Count);

//                if (DETAILED_DFA_STATE_STATS)
//                {
//                    Console.Out.WriteLine("\tMode\tStates\tConfigs\tMode%n");
//                    for (int i = 0; i < modeToDFA.Length; i++)
//                    {
//                        DFA dfa = modeToDFA[i];
//                        if (dfa == null || dfa.states.Count == 0)
//                        {
//                            continue;
//                        }

//                        int modeConfigs = 0;
//                        foreach (DFAState state in dfa.states.Values)
//                        {
//                            modeConfigs += state.configs.Count;
//                        }

//                        String modeName = lexer.getModeNames()[i];
//                        Console.Out.format("\t%d\t%d\t%d\t%s%n", dfa.decision, dfa.states.Count, modeConfigs, modeName);
//                    }
//                }
//            }
//        }

//        if (RUN_PARSER && sharedParsers.Length > 0)
//        {
//            int index = FILE_GRANULARITY ? 0 : ((NumberedThread)Thread.currentThread()).getThreadNumber();
//            Parser parser = sharedParsers[index];
//            // make sure the individual DFAState objects actually have unique ATNConfig arrays
//            ParserATNSimulator interpreter = parser.getInterpreter();
//            DFA[] decisionToDFA = interpreter.decisionToDFA;

//            if (SHOW_DFA_STATE_STATS)
//            {
//                int states = 0;
//                int configs = 0;
//                HashSet<ATNConfig> uniqueConfigs = new HashSet<ATNConfig>();

//                for (int i = 0; i < decisionToDFA.Length; i++)
//                {
//                    DFA dfa = decisionToDFA[i];
//                    if (dfa == null)
//                    {
//                        continue;
//                    }

//                    states += dfa.states.Count;
//                    foreach (DFAState state in dfa.states.Values)
//                    {
//                        configs += state.configs.Count;
//                        uniqueConfigs.UnionWith(state.configs);
//                    }
//                }

//                Console.Out.format("There are %d parser DFAState instances, %d configs (%d unique).%n", states, configs, uniqueConfigs.Count);

//                if (DETAILED_DFA_STATE_STATS)
//                {
//                    if (COMPUTE_TRANSITION_STATS)
//                    {
//                        Console.Out.WriteLine("\tDecision\tStates\tConfigs\tPredict (ALL)\tPredict (LL)\tNon-SLL\tTransitions\tTransitions (ATN)\tTransitions (LL)\tLA (SLL)\tLA (LL)\tRule%n");
//                    }
//                    else
//                    {
//                        Console.Out.WriteLine("\tDecision\tStates\tConfigs\tRule%n");
//                    }

//                    for (int i = 0; i < decisionToDFA.Length; i++)
//                    {
//                        DFA dfa = decisionToDFA[i];
//                        if (dfa == null || dfa.states.Count == 0)
//                        {
//                            continue;
//                        }

//                        int decisionConfigs = 0;
//                        foreach (DFAState state in dfa.states.Values)
//                        {
//                            decisionConfigs += state.configs.Count;
//                        }

//                        String ruleName = parser.getRuleNames()[parser.getATN().decisionToState[(dfa.decision)].ruleIndex];

//                        long calls = 0;
//                        long fullContextCalls = 0;
//                        long nonSllCalls = 0;
//                        long transitions = 0;
//                        long computedTransitions = 0;
//                        long fullContextTransitions = 0;
//                        double lookahead = 0;
//                        double fullContextLookahead = 0;
//                        String formatString;
//                        if (COMPUTE_TRANSITION_STATS)
//                        {
//                            foreach (long[] data in decisionInvocationsPerFile[currentPass])
//                            {
//                                calls += data[i];
//                            }

//                            foreach (long[] data in fullContextFallbackPerFile[currentPass])
//                            {
//                                fullContextCalls += data[i];
//                            }

//                            foreach (long[] data in nonSllPerFile[currentPass])
//                            {
//                                nonSllCalls += data[i];
//                            }

//                            foreach (long[] data in totalTransitionsPerDecisionPerFile[currentPass])
//                            {
//                                transitions += data[i];
//                            }

//                            foreach (long[] data in computedTransitionsPerDecisionPerFile[currentPass])
//                            {
//                                computedTransitions += data[i];
//                            }

//                            foreach (long[] data in fullContextTransitionsPerDecisionPerFile[currentPass])
//                            {
//                                fullContextTransitions += data[i];
//                            }

//                            if (calls > 0)
//                            {
//                                lookahead = (double)(transitions - fullContextTransitions) / (double)calls;
//                            }

//                            if (fullContextCalls > 0)
//                            {
//                                fullContextLookahead = (double)fullContextTransitions / (double)fullContextCalls;
//                            }

//                            formatString = "\t%1$d\t%2$d\t%3$d\t%4$d\t%5$d\t%6$d\t%7$d\t%8$d\t%9$d\t%10$f\t%11$f\t%12$s%n";
//                        }
//                        else
//                        {
//                            calls = 0;
//                            formatString = "\t%1$d\t%2$d\t%3$d\t%12$s%n";
//                        }

//                        Console.Out.format(formatString, dfa.decision, dfa.states.Count, decisionConfigs, calls, fullContextCalls, nonSllCalls, transitions, computedTransitions, fullContextTransitions, lookahead, fullContextLookahead, ruleName);
//                    }
//                }
//            }

//            int localDfaCount = 0;
//            int globalDfaCount = 0;
//            int localConfigCount = 0;
//            int globalConfigCount = 0;
//            int[] contextsInDFAState = new int[0];

//            for (int i = 0; i < decisionToDFA.Length; i++)
//            {
//                DFA dfa = decisionToDFA[i];
//                if (dfa == null)
//                {
//                    continue;
//                }

//                if (SHOW_CONFIG_STATS)
//                {
//                    foreach (DFAState state in dfa.states.Keys)
//                    {
//                        if (state.configs.Count >= contextsInDFAState.Length)
//                        {
//                            contextsInDFAState = Arrays.CopyOf(contextsInDFAState, state.configs.Count + 1);
//                        }

//                        if (state.isAcceptState)
//                        {
//                            bool hasGlobal = false;
//                            foreach (ATNConfig config in state.configs)
//                            {
//                                if (config.reachesIntoOuterContext > 0)
//                                {
//                                    globalConfigCount++;
//                                    hasGlobal = true;
//                                }
//                                else
//                                {
//                                    localConfigCount++;
//                                }
//                            }

//                            if (hasGlobal)
//                            {
//                                globalDfaCount++;
//                            }
//                            else
//                            {
//                                localDfaCount++;
//                            }
//                        }

//                        contextsInDFAState[state.configs.Count]++;
//                    }
//                }
//            }

//            if (SHOW_CONFIG_STATS && currentPass == 0)
//            {
//                Console.Out.format("  DFA accept states: %d total, %d with only local context, %d with a global context%n", localDfaCount + globalDfaCount, localDfaCount, globalDfaCount);
//                Console.Out.format("  Config stats: %d total, %d local, %d global%n", localConfigCount + globalConfigCount, localConfigCount, globalConfigCount);
//                if (SHOW_DFA_STATE_STATS)
//                {
//                    for (int i = 0; i < contextsInDFAState.Length; i++)
//                    {
//                        if (contextsInDFAState[i] != 0)
//                        {
//                            Console.Out.format("  %d configs = %d%n", i, contextsInDFAState[i]);
//                        }
//                    }
//                }
//            }
//        }

//        if (COMPUTE_TIMING_STATS)
//        {
//            Console.Out.format("File\tTokens\tTime%n");
//            for (int i = 0; i < timePerFile[currentPass].Length; i++)
//            {
//                Console.Out.format("%d\t%d\t%d%n", i + 1, tokensPerFile[currentPass][i], timePerFile[currentPass][i]);
//            }
//        }
//    }

    private static long sum(long[] array)
    {
        long result = 0;
        for (int i = 0; i < array.Length; i++)
        {
            result += array[i];
        }

        return result;
    }

    //protected JavaCompiledState compileJavaParser(bool leftRecursive)
    //{
    //    String grammarFileName = leftRecursive ? "JavaLR.g4" : "Java.g4";
    //    String parserName = leftRecursive ? "JavaLRParser" : "JavaParser";
    //    String lexerName = leftRecursive ? "JavaLRLexer" : "JavaLexer";
    //    String body = load(grammarFileName);
    //    List<String> extraOptions = new();
    //    extraOptions.Add("-Werror");
    //    if (FORCE_ATN)
    //    {
    //        extraOptions.Add("-Xforce-atn");
    //    }
    //    if (EXPORT_ATN_GRAPHS)
    //    {
    //        extraOptions.Add("-atn");
    //    }
    //    if (DEBUG_TEMPLATES)
    //    {
    //        extraOptions.Add("-XdbgST");
    //        if (DEBUG_TEMPLATES_WAIT)
    //        {
    //            extraOptions.Add("-XdbgSTWait");
    //        }
    //    }
    //    String[] extraOptionsArray = extraOptions.ToArray();

    //    RunOptions runOptions = createOptionsForJavaToolTests(grammarFileName, body, parserName, lexerName,
    //            false, true, null, null,
    //            false, false, Stage.Compile, false);
    //    RuntimeRunner runner = new JavaRunner();
    //    {
    //        return (JavaCompiledState)runner.run(runOptions);
    //    }
    //}


    private static void updateChecksum(MurmurHashChecksum checksum, int value)
    {
        checksum.update(value);
    }

    private static void updateChecksum(MurmurHashChecksum checksum, Token token)
    {
        if (token == null)
        {
            checksum.update(0);
            return;
        }

        updateChecksum(checksum, token.getStartIndex());
        updateChecksum(checksum, token.getStopIndex());
        updateChecksum(checksum, token.getLine());
        updateChecksum(checksum, token.getCharPositionInLine());
        updateChecksum(checksum, token.getType());
        updateChecksum(checksum, token.getChannel());
    }

    //public class PF : ParserFactory
    //{

    //    //@Override
    //    public FileParseResult parseFile(CharStream input, int currentPass, int thread)
    //    {
    //        MurmurHashChecksum checksum = new MurmurHashChecksum();

    //        long startTime = DateTime.Now.Millisecond;
    //        //assert thread >= 0 && thread < NUMBER_OF_THREADS;

    //        try
    //        {
    //            ParseTreeListener listener = sharedListeners[thread];
    //            if (listener == null)
    //            {
    //                listener = listenerClass.newInstance();
    //                sharedListeners[thread] = listener;
    //            }

    //            Lexer lexer = sharedLexers[thread];
    //            if (REUSE_LEXER && lexer != null)
    //            {
    //                lexer.setInputStream(input);
    //            }
    //            else
    //            {
    //                Lexer previousLexer = lexer;
    //                lexer = lexerCtor.newInstance(input);
    //                DFA[] decisionToDFA = (FILE_GRANULARITY || previousLexer == null ? lexer : previousLexer).getInterpreter().decisionToDFA;
    //                if (!REUSE_LEXER_DFA || (!FILE_GRANULARITY && previousLexer == null))
    //                {
    //                    decisionToDFA = new DFA[decisionToDFA.Length];
    //                }

    //                if (COMPUTE_TRANSITION_STATS)
    //                {
    //                    lexer.setInterpreter(new StatisticsLexerATNSimulator(lexer, lexer.getATN(), decisionToDFA, lexer.getInterpreter().getSharedContextCache()));
    //                }
    //                else if (!REUSE_LEXER_DFA)
    //                {
    //                    lexer.setInterpreter(new LexerATNSimulator(lexer, lexer.getATN(), decisionToDFA, lexer.getInterpreter().getSharedContextCache()));
    //                }

    //                sharedLexers[thread] = lexer;
    //            }

    //            lexer.removeErrorListeners();
    //            lexer.addErrorListener(DescriptiveErrorListener.INSTANCE);

    //            if (lexer.getInterpreter().decisionToDFA[0] == null)
    //            {
    //                ATN atn = lexer.getATN();
    //                for (int i = 0; i < lexer.getInterpreter().decisionToDFA.Length; i++)
    //                {
    //                    lexer.getInterpreter().decisionToDFA[i] = new DFA(atn.getDecisionState(i), i);
    //                }
    //            }

    //            CommonTokenStream tokens = new CommonTokenStream(lexer);
    //            tokens.fill();
    //            tokenCount.addAndGet(currentPass, tokens.Count);

    //            if (COMPUTE_CHECKSUM)
    //            {
    //                foreach (Token token in tokens.getTokens())
    //                {
    //                    updateChecksum(checksum, token);
    //                }
    //            }

    //            if (!RUN_PARSER)
    //            {
    //                return new FileParseResult(input.getSourceName(), (int)checksum.getValue(), null, tokens.size(), startTime, lexer, null);
    //            }

    //            long parseStartTime = DateTime.Now.Nanosecond;
    //            Parser parser = sharedParsers[thread];
    //            if (REUSE_PARSER && parser != null)
    //            {
    //                parser.setInputStream(tokens);
    //            }
    //            else
    //            {
    //                Parser previousParser = parser;

    //                if (USE_PARSER_INTERPRETER)
    //                {
    //                    Parser referenceParser = parserCtor.newInstance(tokens);
    //                    parser = new ParserInterpreter(referenceParser.getGrammarFileName(), referenceParser.getVocabulary(), Arrays.AsList(referenceParser.getRuleNames()), referenceParser.getATN(), tokens);
    //                }
    //                else
    //                {
    //                    parser = parserCtor.newInstance(tokens);
    //                }

    //                DFA[] decisionToDFA = (FILE_GRANULARITY || previousParser == null ? parser : previousParser).getInterpreter().decisionToDFA;
    //                if (!REUSE_PARSER_DFA || (!FILE_GRANULARITY && previousParser == null))
    //                {
    //                    decisionToDFA = new DFA[decisionToDFA.Length];
    //                }

    //                if (COMPUTE_TRANSITION_STATS)
    //                {
    //                    parser.setInterpreter(new StatisticsParserATNSimulator(parser, parser.getATN(), decisionToDFA, parser.getInterpreter().getSharedContextCache()));
    //                }
    //                else if (!REUSE_PARSER_DFA)
    //                {
    //                    parser.setInterpreter(new ParserATNSimulator(parser, parser.getATN(), decisionToDFA, parser.getInterpreter().getSharedContextCache()));
    //                }

    //                sharedParsers[thread] = parser;
    //            }

    //            parser.removeParseListeners();
    //            parser.removeErrorListeners();
    //            if (!TWO_STAGE_PARSING)
    //            {
    //                parser.addErrorListener(DescriptiveErrorListener.INSTANCE);
    //                parser.addErrorListener(new SummarizingDiagnosticErrorListener());
    //            }

    //            if (parser.getInterpreter().decisionToDFA[0] == null)
    //            {
    //                ATN atn = parser.getATN();
    //                for (int i = 0; i < parser.getInterpreter().decisionToDFA.Length; i++)
    //                {
    //                    parser.getInterpreter().decisionToDFA[i] = new DFA(atn.getDecisionState(i), i);
    //                }
    //            }

    //            parser.getInterpreter().setPredictionMode(TWO_STAGE_PARSING ? PredictionMode.SLL : PREDICTION_MODE);
    //            parser.setBuildParseTree(BUILD_PARSE_TREES);
    //            if (!BUILD_PARSE_TREES && BLANK_LISTENER)
    //            {
    //                parser.addParseListener(listener);
    //            }
    //            if (BAIL_ON_ERROR || TWO_STAGE_PARSING)
    //            {
    //                parser.setErrorHandler(new BailErrorStrategy());
    //            }

    //            MethodInfo parseMethod = javaCompiledState.parser.getMethod(entryPoint);
    //            Object parseResult;

    //            try
    //            {
    //                if (COMPUTE_CHECKSUM && !BUILD_PARSE_TREES)
    //                {
    //                    parser.addParseListener(new ChecksumParseTreeListener(checksum));
    //                }

    //                if (USE_PARSER_INTERPRETER)
    //                {
    //                    ParserInterpreter parserInterpreter = (ParserInterpreter)parser;
    //                    parseResult = parserInterpreter.parse(Collections.lastIndexOfSubList(Arrays.AsList(parser.getRuleNames()), Collections.singletonList(entryPoint)));
    //                }
    //                else
    //                {
    //                    parseResult = parseMethod.Invoke(parser);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                if (!TWO_STAGE_PARSING)
    //                {
    //                    throw ex;
    //                }

    //                String sourceName = tokens.getSourceName();
    //                sourceName = sourceName != null && sourceName.Length > 0 ? sourceName + ": " : "";
    //                if (REPORT_SECOND_STAGE_RETRY)
    //                {
    //                    Console.Error.WriteLine(sourceName + "Forced to retry with full context.");
    //                }

    //                if (!(ex.InnerException is ParseCancellationException))
    //                {
    //                    throw ex;
    //                }

    //                tokens.seek(0);
    //                if (REUSE_PARSER && parser != null)
    //                {
    //                    parser.setInputStream(tokens);
    //                }
    //                else
    //                {
    //                    Parser previousParser = parser;

    //                    if (USE_PARSER_INTERPRETER)
    //                    {
    //                        Parser referenceParser = parserCtor.newInstance(tokens);
    //                        parser = new ParserInterpreter(referenceParser.getGrammarFileName(), referenceParser.getVocabulary(), Arrays.AsList(referenceParser.getRuleNames()), referenceParser.getATN(), tokens);
    //                    }
    //                    else
    //                    {
    //                        parser = parserCtor.newInstance(tokens);
    //                    }

    //                    DFA[] decisionToDFA = previousParser.getInterpreter().decisionToDFA;
    //                    if (COMPUTE_TRANSITION_STATS)
    //                    {
    //                        parser.setInterpreter(new StatisticsParserATNSimulator(parser, parser.getATN(), decisionToDFA, parser.getInterpreter().getSharedContextCache()));
    //                    }
    //                    else if (!REUSE_PARSER_DFA)
    //                    {
    //                        parser.setInterpreter(new ParserATNSimulator(parser, parser.getATN(), decisionToDFA, parser.getInterpreter().getSharedContextCache()));
    //                    }

    //                    sharedParsers[thread] = parser;
    //                }

    //                parser.removeParseListeners();
    //                parser.removeErrorListeners();
    //                parser.addErrorListener(DescriptiveErrorListener.INSTANCE);
    //                parser.addErrorListener(new SummarizingDiagnosticErrorListener());
    //                parser.getInterpreter().setPredictionMode(PredictionMode.LL);
    //                parser.setBuildParseTree(BUILD_PARSE_TREES);
    //                if (COMPUTE_CHECKSUM && !BUILD_PARSE_TREES)
    //                {
    //                    parser.addParseListener(new ChecksumParseTreeListener(checksum));
    //                }
    //                if (!BUILD_PARSE_TREES && BLANK_LISTENER)
    //                {
    //                    parser.addParseListener(listener);
    //                }
    //                if (BAIL_ON_ERROR)
    //                {
    //                    parser.setErrorHandler(new BailErrorStrategy());
    //                }

    //                parseResult = parseMethod.Invoke(parser);
    //            }

    //            Assert.IsTrue(parseResult is ParseTree);
    //            if (COMPUTE_CHECKSUM && BUILD_PARSE_TREES)
    //            {
    //                ParseTreeWalker.DEFAULT.walk(new ChecksumParseTreeListener(checksum), (ParseTree)parseResult);
    //            }
    //            if (BUILD_PARSE_TREES && BLANK_LISTENER)
    //            {
    //                ParseTreeWalker.DEFAULT.walk(listener, (ParseTree)parseResult);
    //            }

    //            return new FileParseResult(input.getSourceName(), (int)checksum.getValue(), (ParseTree)parseResult, tokens.size(), TIME_PARSE_ONLY ? parseStartTime : startTime, lexer, parser);
    //        }
    //        catch (Exception e)
    //        {
    //            if (!REPORT_SYNTAX_ERRORS && e is ParseCancellationException)
    //            {
    //                return new FileParseResult("unknown", (int)checksum.getValue(), null, 0, startTime, null, null);
    //            }

    //            //e.printStackTrace(Console.Out);
    //            throw new IllegalStateException(e.Message,e);
    //        }
    //    }
    //}
    //protected ParserFactory getParserFactory(JavaCompiledState javaCompiledState, String listenerName, String entryPoint)
    //{
    //    try
    //    {
    //        Type listenerClass = Type.GetType(listenerName);

    //        ConstructorInfo lexerCtor = javaCompiledState.lexerType.GetConstructor(CharStream);
    //        ConstructorInfo parserCtor = javaCompiledState.parserType.GetConstructor(TokenStream);

    //        // construct initial instances of the lexer and parser to deserialize their ATNs
    //        javaCompiledState.initializeLexerAndParser("");

    //        return new PF();
    //    }
    //    catch (Exception e)
    //    {
    //        //e.printStackTrace(Console.Out);
    //        Assert.Fail(e.Message);
    //        throw new IllegalStateException(e);
    //    }
    //}

    public interface ParserFactory
    {
        FileParseResult parseFile(CharStream input, int currentPass, int thread);
    }

    public class FileParseResult
    {
        public readonly String sourceName;
        public readonly int checksum;
        public readonly ParseTree parseTree;
        public readonly int tokenCount;
        public readonly long startTime;
        public readonly long endTime;

        public readonly int lexerDFASize;
        public readonly long lexerTotalTransitions;
        public readonly long lexerComputedTransitions;

        public readonly int parserDFASize;
        public readonly long[] decisionInvocations;
        public readonly long[] fullContextFallback;
        public readonly long[] nonSll;
        public readonly long[] parserTotalTransitions;
        public readonly long[] parserComputedTransitions;
        public readonly long[] parserFullContextTransitions;

        public FileParseResult(String sourceName, int checksum, ParseTree parseTree, int tokenCount, long startTime, Lexer lexer, Parser parser)
        {
            this.sourceName = sourceName;
            this.checksum = checksum;
            this.parseTree = parseTree;
            this.tokenCount = tokenCount;
            this.startTime = startTime;
            this.endTime = DateTime.Now.Nanosecond;

            if (lexer != null)
            {
                LexerATNSimulator interpreter = lexer.getInterpreter();
                if (interpreter is StatisticsLexerATNSimulator)
                {
                    lexerTotalTransitions = ((StatisticsLexerATNSimulator)interpreter).totalTransitions;
                    lexerComputedTransitions = ((StatisticsLexerATNSimulator)interpreter).computedTransitions;
                }
                else
                {
                    lexerTotalTransitions = 0;
                    lexerComputedTransitions = 0;
                }

                int dfaSize = 0;
                foreach (DFA dfa in interpreter.decisionToDFA)
                {
                    if (dfa != null)
                    {
                        dfaSize += dfa.states.Count;
                    }
                }

                lexerDFASize = dfaSize;
            }
            else
            {
                lexerDFASize = 0;
                lexerTotalTransitions = 0;
                lexerComputedTransitions = 0;
            }

            if (parser != null)
            {
                ParserATNSimulator interpreter = parser.getInterpreter();
                if (interpreter is StatisticsParserATNSimulator)
                {
                    decisionInvocations = ((StatisticsParserATNSimulator)interpreter).decisionInvocations;
                    fullContextFallback = ((StatisticsParserATNSimulator)interpreter).fullContextFallback;
                    nonSll = ((StatisticsParserATNSimulator)interpreter).nonSll;
                    parserTotalTransitions = ((StatisticsParserATNSimulator)interpreter).totalTransitions;
                    parserComputedTransitions = ((StatisticsParserATNSimulator)interpreter).computedTransitions;
                    parserFullContextTransitions = ((StatisticsParserATNSimulator)interpreter).fullContextTransitions;
                }
                else
                {
                    decisionInvocations = new long[0];
                    fullContextFallback = new long[0];
                    nonSll = new long[0];
                    parserTotalTransitions = new long[0];
                    parserComputedTransitions = new long[0];
                    parserFullContextTransitions = new long[0];
                }

                int dfaSize = 0;
                foreach (DFA dfa in interpreter.decisionToDFA)
                {
                    if (dfa != null)
                    {
                        dfaSize += dfa.states.Count;
                    }
                }

                parserDFASize = dfaSize;
            }
            else
            {
                parserDFASize = 0;
                decisionInvocations = new long[0];
                fullContextFallback = new long[0];
                nonSll = new long[0];
                parserTotalTransitions = new long[0];
                parserComputedTransitions = new long[0];
                parserFullContextTransitions = new long[0];
            }
        }
    }

    private class StatisticsLexerATNSimulator : LexerATNSimulator
    {

        public long totalTransitions;
        public long computedTransitions;

        public StatisticsLexerATNSimulator(ATN atn, DFA[] decisionToDFA, PredictionContextCache sharedContextCache)
        : base(atn, decisionToDFA, sharedContextCache)
        {
            ;
        }

        public StatisticsLexerATNSimulator(Lexer recog, ATN atn, DFA[] decisionToDFA, PredictionContextCache sharedContextCache)
        : base(recog, atn, decisionToDFA, sharedContextCache)
        {
            ;
        }

        //@Override
        protected DFAState getExistingTargetState(DFAState s, int t)
        {
            totalTransitions++;
            return base.getExistingTargetState(s, t);
        }

        //@Override
        protected DFAState computeTargetState(CharStream input, DFAState s, int t)
        {
            computedTransitions++;
            return base.computeTargetState(input, s, t);
        }
    }

    private class StatisticsParserATNSimulator : ParserATNSimulator
    {

        public readonly long[] decisionInvocations;
        public readonly long[] fullContextFallback;
        public readonly long[] nonSll;
        public readonly long[] totalTransitions;
        public readonly long[] computedTransitions;
        public readonly long[] fullContextTransitions;

        private int decision;

        public StatisticsParserATNSimulator(ATN atn, DFA[] decisionToDFA, PredictionContextCache sharedContextCache)
        : base(atn, decisionToDFA, sharedContextCache)
        {
            ;
            decisionInvocations = new long[atn.decisionToState.Count];
            fullContextFallback = new long[atn.decisionToState.Count];
            nonSll = new long[atn.decisionToState.Count];
            totalTransitions = new long[atn.decisionToState.Count];
            computedTransitions = new long[atn.decisionToState.Count];
            fullContextTransitions = new long[atn.decisionToState.Count];
        }

        public StatisticsParserATNSimulator(Parser parser, ATN atn, DFA[] decisionToDFA, PredictionContextCache sharedContextCache)
        : base(parser, atn, decisionToDFA, sharedContextCache)
        {
            decisionInvocations = new long[atn.decisionToState.Count];
            fullContextFallback = new long[atn.decisionToState.Count];
            nonSll = new long[atn.decisionToState.Count];
            totalTransitions = new long[atn.decisionToState.Count];
            computedTransitions = new long[atn.decisionToState.Count];
            fullContextTransitions = new long[atn.decisionToState.Count];
        }

        //@Override
        public int adaptivePredict(TokenStream input, int decision, ParserRuleContext outerContext)
        {
            try
            {
                this.decision = decision;
                decisionInvocations[decision]++;
                return base.adaptivePredict(input, decision, outerContext);
            }
            finally
            {
                this.decision = -1;
            }
        }

        //@Override
        protected int execATNWithFullContext(DFA dfa, DFAState D, ATNConfigSet s0, TokenStream input, int startIndex, ParserRuleContext outerContext)
        {
            fullContextFallback[decision]++;
            return base.execATNWithFullContext(dfa, D, s0, input, startIndex, outerContext);
        }

        //@Override
        protected DFAState getExistingTargetState(DFAState previousD, int t)
        {
            totalTransitions[decision]++;
            return base.getExistingTargetState(previousD, t);
        }

        //@Override
        protected DFAState computeTargetState(DFA dfa, DFAState previousD, int t)
        {
            computedTransitions[decision]++;
            return base.computeTargetState(dfa, previousD, t);
        }

        //@Override
        protected ATNConfigSet computeReachSet(ATNConfigSet closure, int t, bool fullCtx)
        {
            if (fullCtx)
            {
                totalTransitions[decision]++;
                computedTransitions[decision]++;
                fullContextTransitions[decision]++;
            }

            return base.computeReachSet(closure, t, fullCtx);
        }
    }

    private class DescriptiveErrorListener : BaseErrorListener
    {
        public readonly static DescriptiveErrorListener INSTANCE = new DescriptiveErrorListener();

        //@Override
        public void syntaxError(Recognizer<Token, ATNSimulator> recognizer, Object offendingSymbol,
                                int line, int charPositionInLine,
                                String msg, RecognitionException e)
        {
            if (!REPORT_SYNTAX_ERRORS)
            {
                return;
            }

            String sourceName = recognizer.getInputStream().getSourceName();
            if (sourceName.Length > 0)
            {
                sourceName = $"{sourceName}:{line}:{charPositionInLine}: ";//, sourceName, line, charPositionInLine);
            }

            Console.Error.WriteLine(sourceName + "line " + line + ":" + charPositionInLine + " " + msg);
        }

    }

    private class SummarizingDiagnosticErrorListener : DiagnosticErrorListener
    {
        private BitSet _sllConflict;
        private ATNConfigSet _sllConfigs;

        //@Override
        public void reportAmbiguity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact, BitSet ambigAlts, ATNConfigSet configs)
        {
            if (COMPUTE_TRANSITION_STATS && DETAILED_DFA_STATE_STATS)
            {
                BitSet sllPredictions = getConflictingAlts(_sllConflict, _sllConfigs);
                int sllPrediction = sllPredictions.NextSetBit(0);
                BitSet llPredictions = getConflictingAlts(ambigAlts, configs);
                int llPrediction = llPredictions.Cardinality() == 0 ? ATN.INVALID_ALT_NUMBER : llPredictions.NextSetBit(0);
                if (sllPrediction != llPrediction)
                {
                    ((StatisticsParserATNSimulator)recognizer.getInterpreter()).nonSll[dfa.decision]++;
                }
            }

            if (!REPORT_AMBIGUITIES)
            {
                return;
            }

            // show the rule name along with the decision
            int decision = dfa.decision;
            String rule = recognizer.getRuleNames()[dfa.atnStartState.ruleIndex];
            String input = recognizer.getTokenStream().getText(Interval.of(startIndex, stopIndex));
            String format = $"reportAmbiguity d={decision} ({rule}): ambigAlts={ambigAlts}, input='{input}'";
            recognizer.notifyErrorListeners(format);
        }

        //@Override
        public void reportAttemptingFullContext(Parser recognizer, DFA dfa, int startIndex, int stopIndex, BitSet conflictingAlts, ATNConfigSet configs)
        {
            _sllConflict = conflictingAlts;
            _sllConfigs = configs;
            if (!REPORT_FULL_CONTEXT)
            {
                return;
            }

            // show the rule name and viable configs along with the base info
            int decision = dfa.decision;
            String rule = recognizer.getRuleNames()[dfa.atnStartState.ruleIndex];
            String input = recognizer.getTokenStream().getText(Interval.of(startIndex, stopIndex));
            BitSet representedAlts = getConflictingAlts(conflictingAlts, configs);
            String format = $"reportAttemptingFullContext d={decision} ({rule}), input='{input}', viable={representedAlts}";
            //String.Format(format, decision, rule, input, representedAlts)
            recognizer.notifyErrorListeners(format);
        }

        //@Override
        public void reportContextSensitivity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, int prediction, ATNConfigSet configs)
        {
            if (COMPUTE_TRANSITION_STATS && DETAILED_DFA_STATE_STATS)
            {
                BitSet sllPredictions = getConflictingAlts(_sllConflict, _sllConfigs);
                int sllPrediction = sllPredictions.NextSetBit(0);
                if (sllPrediction != prediction)
                {
                    ((StatisticsParserATNSimulator)recognizer.getInterpreter()).nonSll[dfa.decision]++;
                }
            }

            if (!REPORT_CONTEXT_SENSITIVITY)
            {
                return;
            }

            // show the rule name and viable configs along with the base info
            int decision = dfa.decision;
            String rule = recognizer.getRuleNames()[dfa.atnStartState.ruleIndex];
            String input = recognizer.getTokenStream().getText(Interval.of(startIndex, stopIndex));
            String format = $"reportContextSensitivity d={decision} ({rule}), input='{input}', viable={prediction}";
            recognizer.notifyErrorListeners(
                format);
        }

    }
    public class NumberedThread
    {
        private readonly int threadNumber;

        public NumberedThread(Runnable target, int threadNumber)
        {
            ;
            this.threadNumber = threadNumber;
        }

        public int getThreadNumber()
        {
            return threadNumber;
        }

    }

    protected class NumberedThreadFactory
    {
        private readonly AtomicInteger nextThread = new AtomicInteger();

        //@Override
        public NumberedThread newThread(Runnable r)
        {
            int threadNumber = nextThread.getAndIncrement();
            //assert threadNumber<NUMBER_OF_THREADS;
            return new NumberedThread(r, threadNumber);
        }

    }

    public class FixedThreadNumberFactory
    {
        private readonly int threadNumber;

        public FixedThreadNumberFactory(int threadNumber)
        {
            this.threadNumber = threadNumber;
        }

        //@Override
        public NumberedThread newThread(Runnable r)
        {
            //assert threadNumber<NUMBER_OF_THREADS;
            return new NumberedThread(r, threadNumber);
        }
    }

    protected class ChecksumParseTreeListener : ParseTreeListener
    {
        private static readonly int VISIT_TERMINAL = 1;
        private static readonly int VISIT_ERROR_NODE = 2;
        private static readonly int ENTER_RULE = 3;
        private static readonly int EXIT_RULE = 4;

        private readonly MurmurHashChecksum checksum;

        public ChecksumParseTreeListener(MurmurHashChecksum checksum)
        {
            this.checksum = checksum;
        }

        //@Override
        public void visitTerminal(TerminalNode node)
        {
            checksum.update(VISIT_TERMINAL);
            updateChecksum(checksum, node.getSymbol());
        }

        //@Override
        public void visitErrorNode(ErrorNode node)
        {
            checksum.update(VISIT_ERROR_NODE);
            updateChecksum(checksum, node.getSymbol());
        }

        //@Override
        public void enterEveryRule(ParserRuleContext ctx)
        {
            checksum.update(ENTER_RULE);
            updateChecksum(checksum, ctx.getRuleIndex());
            updateChecksum(checksum, ctx.getStart());
        }

        //@Override
        public void exitEveryRule(ParserRuleContext ctx)
        {
            checksum.update(EXIT_RULE);
            updateChecksum(checksum, ctx.getRuleIndex());
            updateChecksum(checksum, ctx.getStop());
        }

    }

    public class InputDescriptor
    {
        private readonly String source;
        private Reference<CloneableANTLRFileStream> inputStream;

        public InputDescriptor(String source)
        {
            this.source = source;
            if (PRELOAD_SOURCES)
            {
                getInputStream();
            }
        }


        public /*synchronized*/ CharStream getInputStream()
        {
            CloneableANTLRFileStream stream = inputStream != null ? inputStream.get() : null;
            if (stream == null)
            {
                try
                {
                    stream = new CloneableANTLRFileStream(source, ENCODING);
                }
                catch (IOException ex)
                {
                    throw new RuntimeException(ex.Message,ex);
                }

                if (PRELOAD_SOURCES)
                {
                    inputStream = new StrongReference<CloneableANTLRFileStream>(stream);
                }
                else
                {
                    inputStream = new SoftReference<CloneableANTLRFileStream>(stream);
                }
            }

            return new JavaUnicodeInputStream(stream.createCopy());
        }
    }

    protected class CloneableANTLRFileStream : ANTLRFileStream
    {

        public CloneableANTLRFileStream(String fileName, Encoding encoding)
            : base(fileName, encoding)
        {
        }

        public ANTLRInputStream createCopy()
        {
            ANTLRInputStream stream = new ANTLRInputStream(this.data, this.n);
            stream.name = this.getSourceName();
            return stream;
        }
    }
    public interface Reference<T>
    {
        T get();
    }
    public class StrongReference<T>:Reference<T> 
    {
        public readonly T referent;

        public StrongReference(T referent)
        {
            ;
            this.referent = referent;
        }

        //@Override
        public T get()
        {
            return referent;
        }
    }
    public class SoftReference<T> : Reference<T>
    {
        public readonly T referent;

        public SoftReference(T referent)
        {
            ;
            this.referent = referent;
        }

        //@Override
        public T get()
        {
            return referent;
        }
    }


    public class MurmurHashChecksum
    {
        private int value;
        private int count;

        public MurmurHashChecksum()
        {
            this.value = MurmurHash.initialize();
        }

        public void update(int value)
        {
            this.value = MurmurHash.update(this.value, value);
            this.count++;
        }

        public int getValue()
        {
            return MurmurHash.finish(value, count);
        }
    }

    [TestMethod]
    //@Timeout(20)
    public void testExponentialInclude(string tempDir)
    {
        String tempDirPath = tempDir.ToString();
        String grammarFormat =
            "parser grammar Level_{0}_{1};\n" +
            "\n" +
            "{2} import Level_{3}_1, Level_{4}_2;\n" +
            "\n" +
            "rule_{5}_{6} : EOF;\n";

        FileUtils.mkdir(tempDirPath);

        long startTime = DateTime.Now.Nanosecond;

        int levels = 20;
        for (int level = 0; level < levels; level++)
        {
            String leafPrefix = level == levels - 1 ? "//" : "";
            String grammar1 = String.Format(grammarFormat, level, 1, leafPrefix, level + 1, level + 1, level, 1);
                FileUtils.writeFile(tempDirPath, "Level_" + level + "_1.g4", grammar1);
            if (level > 0)
            {
                String grammar2 = String.Format(grammarFormat, level, 2, leafPrefix, level + 1, level + 1, level, 1);
                FileUtils.writeFile(tempDirPath, "Level_" + level + "_2.g4", grammar2);
            }
        }

        ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "Level_0_1.g4", false);
        Assert.IsTrue(equeue.errors.Count == 0);

        long endTime = DateTime.Now.Nanosecond;
        Console.Out.WriteLine("{(endTime - startTime) / 1000000.0} milliseconds.%n" );
    }
}
