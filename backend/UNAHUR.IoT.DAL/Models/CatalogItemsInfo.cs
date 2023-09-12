﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UNAHUR.IoT.DAL.MOdels;

[Keyless]
public partial class CatalogItemsInfo
{
    public long CatalogItemId { get; set; }

    public int PartitionId { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Description { get; set; }

    [Required]
    public HierarchyId PartitionLevel { get; set; }
}