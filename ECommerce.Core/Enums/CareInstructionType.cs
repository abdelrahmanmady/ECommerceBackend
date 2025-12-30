namespace ECommerce.Core.Enums
{
    public enum CareInstructionType : byte
    {
        // Washing
        MachineWashCold,
        MachineWashWarm,
        HandWash,
        DoNotWash,

        // Bleaching
        DoNotBleach,
        BleachAny,

        // Drying
        TumbleDryLow,
        TumbleDryHigh,
        DoNotTumbleDry,
        DryFlat,

        // Ironing
        IronLow,
        IronMedium,
        IronHigh,
        DoNotIron,

        // Professional Care
        DryCleanOnly,
        DoNotDryClean
    }
}
