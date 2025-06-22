using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    const int searchDepth = 4;
    static readonly int[] pieceValues = { 0, 100, 320, 330, 500, 900, 20000 };

    static readonly int[] pawnTable = new int[64] {
         0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
         5,  5, 10, 25, 25, 10,  5,  5,
         0,  0,  0, 20, 20,  0,  0,  0,
         5, -5,-10,  0,  0,-10, -5,  5,
         5, 10, 10,-20,-20, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0 };

    static readonly int[] knightTable = new int[64] {
       -50,-40,-30,-30,-30,-30,-40,-50,
       -40,-20,  0,  0,  0,  0,-20,-40,
       -30,  0, 10, 15, 15, 10,  0,-30,
       -30,  5, 15, 20, 20, 15,  5,-30,
       -30,  0, 15, 20, 20, 15,  0,-30,
       -30,  5, 10, 15, 15, 10,  5,-30,
       -40,-20,  0,  5,  5,  0,-20,-40,
       -50,-40,-30,-30,-30,-30,-40,-50 };

    static readonly int[] bishopTable = new int[64] {
       -20,-10,-10,-10,-10,-10,-10,-20,
       -10,  0,  0,  0,  0,  0,  0,-10,
       -10,  0,  5, 10, 10,  5,  0,-10,
       -10,  5,  5, 10, 10,  5,  5,-10,
       -10,  0, 10, 10, 10, 10,  0,-10,
       -10, 10, 10, 10, 10, 10, 10,-10,
       -10,  5,  0,  0,  0,  0,  5,-10,
       -20,-10,-10,-10,-10,-10,-10,-20 };

    static readonly int[] rookTable = new int[64] {
         0,  0,  0,  5,  5,  0,  0,  0,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
         5, 10, 10, 10, 10, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0 };

    static readonly int[] queenTable = new int[64] {
       -20,-10,-10, -5, -5,-10,-10,-20,
       -10,  0,  0,  0,  0,  0,  0,-10,
       -10,  0,  5,  5,  5,  5,  0,-10,
        -5,  0,  5,  5,  5,  5,  0, -5,
         0,  0,  5,  5,  5,  5,  0, -5,
       -10,  5,  5,  5,  5,  5,  0,-10,
       -10,  0,  5,  0,  0,  0,  0,-10,
       -20,-10,-10, -5, -5,-10,-10,-20 };

    static readonly int[] kingTable = new int[64] {
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -20,-30,-30,-40,-40,-30,-30,-20,
       -10,-20,-20,-20,-20,-20,-20,-10,
        20, 20,  0,  0,  0,  0, 20, 20,
        20, 30, 10,  0,  0, 10, 30, 20 };

    static readonly int[][] pst = { null, pawnTable, knightTable, bishopTable, rookTable, queenTable, kingTable };
    Move bestMove;

    public Move Think(Board board, Timer timer)
    {
        bestMove = board.GetLegalMoves()[0];
        AlphaBeta(board, searchDepth, int.MinValue + 1, int.MaxValue - 1);
        return bestMove;
    }

    int AlphaBeta(Board board, int depth, int alpha, int beta)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
            return Evaluate(board);

        Move[] moves = board.GetLegalMoves();
        if (moves.Length == 0)
            return Evaluate(board);

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -AlphaBeta(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);

            if (score >= beta)
                return beta;
            if (score > alpha)
            {
                alpha = score;
                if (depth == searchDepth)
                    bestMove = move;
            }
        }
        return alpha;
    }

    int Evaluate(Board board)
    {
        int eval = EvaluateForWhite(board);
        return board.IsWhiteToMove ? eval : -eval;
    }

    int EvaluateForWhite(Board board)
    {
        if (board.IsInCheckmate())
            return board.IsWhiteToMove ? -100000 : 100000;
        if (board.IsDraw())
            return 0;

        int score = 0;
        score += EvaluateMaterial(board);
        score += EvaluatePieceSquares(board);
        score += 5 * EvaluateMobility(board);
        return score;
    }

    int EvaluateMaterial(Board board)
    {
        int score = 0;
        for (int i = 1; i <= 5; i++)
            score += pieceValues[i] * board.GetPieceList((PieceType)i, true).Count;
        for (int i = 1; i <= 5; i++)
            score -= pieceValues[i] * board.GetPieceList((PieceType)i, false).Count;
        return score;
    }

    int EvaluatePieceSquares(Board board)
    {
        int score = 0;
        for (int i = 1; i <= 6; i++)
        {
            foreach (var p in board.GetPieceList((PieceType)i, true))
                score += pst[i][p.Square.Index];
            foreach (var p in board.GetPieceList((PieceType)i, false))
                score -= pst[i][63 - p.Square.Index];
        }
        return score;
    }

    int EvaluateMobility(Board board)
    {
        int whiteMoves, blackMoves;
        if (board.IsWhiteToMove)
        {
            whiteMoves = board.GetLegalMoves().Length;
            board.ForceSkipTurn();
            blackMoves = board.GetLegalMoves().Length;
            board.UndoSkipTurn();
        }
        else
        {
            blackMoves = board.GetLegalMoves().Length;
            board.ForceSkipTurn();
            whiteMoves = board.GetLegalMoves().Length;
            board.UndoSkipTurn();
        }
        return whiteMoves - blackMoves;
    }
}
