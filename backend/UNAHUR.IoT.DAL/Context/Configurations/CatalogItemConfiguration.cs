﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using UNAHUR.IoT.DAL.Context;
using UNAHUR.IoT.DAL.MOdels;

namespace UNAHUR.IoT.DAL.Context.Configurations
{
    public partial class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> entity)
        {
            entity.Property(e => e.CatalogItemId).HasComment("Unique identifier");
            entity.Property(e => e.Description).HasComment("descripcion larga");
            entity.Property(e => e.LastModified)
            .HasDefaultValueSql("(getdate())")
            .HasComment("Fecha de ultima modificacion");
            entity.Property(e => e.LastModifiedBy)
            .HasDefaultValueSql("(suser_sname())")
            .HasComment("Usuario de ultima modificacion");
            entity.Property(e => e.Name).HasComment("Nombre para mostrar");
            entity.Property(e => e.PartitionId).HasComment("Partition ID");

            entity.HasOne(d => d.Partition).WithMany(p => p.CatalogItems)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CatalogItems_Partitions");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<CatalogItem> entity);
    }
}