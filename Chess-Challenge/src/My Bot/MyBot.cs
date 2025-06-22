using ChessChallenge.API;

public class MyBot : IChessBot
{
    const int d = 4;
    static int[] v = {0,100,320,330,500,900,20000};

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

    static int[][] S = { null,P,N,B,R,Q,K };
    Move m;

    public Move Think(Board board, Timer t)
    {
        m = board.GetLegalMoves()[0];
        Search(board,d,int.MinValue+1,int.MaxValue-1);
        return m;
    }

    int Search(Board b,int depth,int a,int beta)
    {
        if(depth==0||b.IsInCheckmate()||b.IsDraw())
            return Eval(b);

        Move[] moves = b.GetLegalMoves();
        if(moves.Length==0)
            return Eval(b);

        foreach(Move mv in moves)
        {
            b.MakeMove(mv);
            int score=-Search(b,depth-1,-beta,-a);
            b.UndoMove(mv);

            if(score>=beta)
                return beta;
            if(score>a)
            {
                a=score;
                if(depth==d)
                    m=mv;
            }
        }
        return a;
    }

    int Eval(Board b)
    {
        if(b.IsInCheckmate())
            return b.IsWhiteToMove?-100000:100000;
        if(b.IsDraw())
            return 0;
        int score=0;
        for(int i=1;i<=6;i++)
        {
            foreach(var pce in b.GetPieceList((PieceType)i,true))
            {
                if(i<6)score+=v[i];
                score+=S[i][pce.Square.Index];
            }
            foreach(var pce in b.GetPieceList((PieceType)i,false))
            {
                if(i<6)score-=v[i];
                score-=S[i][63-pce.Square.Index];
            }
        }
        b.ForceSkipTurn();
        int opp=b.GetLegalMoves().Length;
        b.UndoSkipTurn();
        int me=b.GetLegalMoves().Length;
        score+=5*(me-opp);
        return b.IsWhiteToMove?score:-score;
    }

}
