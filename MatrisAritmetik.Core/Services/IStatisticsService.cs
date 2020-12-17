using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IStatisticsService
    {
        Dataframe Head(int n);

        Dataframe Tail(int r);

        float Sum(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Sum(Dataframe df);

        float Mul(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Mul(Dataframe df);

        float Min(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Min(Dataframe df);

        float Max(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Max(Dataframe df);

        float Mean(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Mean(Dataframe df);

        float Mode(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Mode(Dataframe df);

        float Median(Dataframe df, int index, int axis = 1, int based = 0);

        Dataframe Median(Dataframe df);

        float SDev(Dataframe df, int index, int axis = 1, int usePopulation = 0, int based = 0);

        Dataframe SDev(Dataframe df, int usePopulation = 0);

    }
}
