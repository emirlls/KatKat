using System;
using KatKat.Enums;

namespace KatKat.Dtos;

public class ReportSosAlertDto
{
    public Guid ComplexId { get; set; }

    public Guid FlatId { get; set; }

    public SosStatuses Status { get; set; }
}
