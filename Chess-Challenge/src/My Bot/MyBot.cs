using ChessChallenge.API;

// CRAZYHORSEBOT
public class MyBot : IChessBot
{
    const int d = 4;
    static int[] v = { 0, 100, 320, 330, 500, 900, 20000 };

    static int[] P = new int[64] {
         0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
         5,  5, 10, 25, 25, 10,  5,  5,
         0,  0,  0, 20, 20,  0,  0,  0,
         5, -5,-10,  0,  0,-10, -5,  5,
         5, 10, 10,-20,-20, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0 };

    static int[] N = new int[64] {
       -50,-40,-30,-30,-30,-30,-40,-50,
       -40,-20,  0,  0,  0,  0,-20,-40,
       -30,  0, 10, 15, 15, 10,  0,-30,
       -30,  5, 15, 20, 20, 15,  5,-30,
       -30,  0, 15, 20, 20, 15,  0,-30,
       -30,  5, 10, 15, 15, 10,  5,-30,
       -40,-20,  0,  5,  5,  0,-20,-40,
       -50,-40,-30,-30,-30,-30,-40,-50 };

    static int[] B = new int[64] {
       -20,-10,-10,-10,-10,-10,-10,-20,
       -10,  0,  0,  0,  0,  0,  0,-10,
       -10,  0,  5, 10, 10,  5,  0,-10,
       -10,  5,  5, 10, 10,  5,  5,-10,
       -10,  0, 10, 10, 10, 10,  0,-10,
       -10, 10, 10, 10, 10, 10, 10,-10,
       -10,  5,  0,  0,  0,  0,  5,-10,
       -20,-10,-10,-10,-10,-10,-10,-20 };

    static int[] R = new int[64] {
         0,  0,  0,  5,  5,  0,  0,  0,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
         5, 10, 10, 10, 10, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0 };

    static int[] Q = new int[64] {
       -20,-10,-10, -5, -5,-10,-10,-20,
       -10,  0,  0,  0,  0,  0,  0,-10,
       -10,  0,  5,  5,  5,  5,  0,-10,
        -5,  0,  5,  5,  5,  5,  0, -5,
         0,  0,  5,  5,  5,  5,  0, -5,
       -10,  5,  5,  5,  5,  5,  0,-10,
       -10,  0,  5,  0,  0,  0,  0,-10,
       -20,-10,-10, -5, -5,-10,-10,-20 };

    static int[] K = new int[64] {
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -20,-30,-30,-40,-40,-30,-30,-20,
       -10,-20,-20,-20,-20,-20,-20,-10,
        20, 20,  0,  0,  0,  0, 20, 20,
        20, 30, 10,  0,  0, 10, 30, 20 };

    static int[][] S = { null, P, N, B, R, Q, K };
    Move m;

    public Move Think(Board board, Timer t)
    {
        m = board.GetLegalMoves()[0];
        Search(board, d, int.MinValue + 1, int.MaxValue - 1);
        return m;
    }

    int Search(Board board, int depth, int alpha, int beta)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
            return Eval(board);

        Move[] moves = board.GetLegalMoves();
        if (moves.Length == 0)
            return Eval(board);

        foreach (Move mv in moves)
        {
            board.MakeMove(mv);
            int score = -Search(board, depth - 1, -beta, -alpha);
            board.UndoMove(mv);

            if (score >= beta)
                return beta;
            if (score > alpha)
            {
                alpha = score;
                if (depth == d)
                    m = mv;
            }
        }
        return alpha;
    }

    int Eval(Board board)
    {
        if (board.IsInCheckmate())
            return board.IsWhiteToMove ? -100000 : 100000;
        if (board.IsDraw())
            return 0;
        int score = 0;
        for (int i = 1; i <= 6; i++)
        {
            foreach (var pce in board.GetPieceList((PieceType)i, true))
            {
                if (i < 6) score += v[i];
                score += S[i][pce.Square.Index];
            }
            foreach (var pce in board.GetPieceList((PieceType)i, false))
            {
                if (i < 6) score -= v[i];
                score -= S[i][63 - pce.Square.Index];
            }
        }
        board.ForceSkipTurn();
        int opp = board.GetLegalMoves().Length;
        board.UndoSkipTurn();
        int me = board.GetLegalMoves().Length;
        score += 5 * (me - opp);
        return board.IsWhiteToMove ? score : -score;
    }

}