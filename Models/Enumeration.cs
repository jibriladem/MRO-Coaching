using System.ComponentModel;

namespace MROCoatching.DataObjects.Models
{
    public enum RoomName
    {
        Superior = 1,
        Delux,
        Executive,
        [Description("Superior Suite")]
        SuperiorSuite,
        [Description("Corner Suite")]
        CornerSuite,
        [Description("Executive Suite")]
        ExecutiveSuite,
        [Description("Grand Suite")]
        GrandSuite,
        [Description("Presidential Suite")]
        PresidentialSuite
    }

    public enum CustomerType
    {
        Adult = 1,
        Child
    }

    public enum RoomStatus
    {
        [Description("Sold Out")]
        SoldOut = 1,
        Available
    }
    public enum ReservationStatus
    {
        [Description("Non-Guaranteed")]
        NonGuaranteed = 1,
        Confirmed,
        [Description("Non-Show")]
        NonShow,
        Canceled
    }


    public enum OperationStatus
    {
        ERROR,
        OK,
        WARNING,
        SUCCESS,
        NOT_OK,
    }

    public enum Status
    {
        Active,
        Inactive
    }

}
