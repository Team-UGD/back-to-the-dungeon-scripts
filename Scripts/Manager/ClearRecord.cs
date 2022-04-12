using System;

[Serializable]
public struct ClearRecord
{
    private DateTime date;
    private double clearTime;

    public DateTime Date { get => date; set => date = value; }
    public double ClearTime { get => clearTime; set => clearTime = value < 0d ? 0d : value; }

    public ClearRecord(DateTime date, double clearTime)
    {
        this.date = date;
        this.clearTime = clearTime < 0d ? 0d : clearTime;
    }

    public override string ToString()
    {
        return $"Clear Record {{ Date = {this.date}, Clear Time = {this.clearTime:0.00} }}";
    }
}