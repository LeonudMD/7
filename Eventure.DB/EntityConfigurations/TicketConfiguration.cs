using Eventure.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.DB.EntityConfigurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<TicketEntity>
    {
        public void Configure(EntityTypeBuilder<TicketEntity> builder)
        {
            // Установка ключа
            builder.HasKey(x => x.Id);

            // Конфигурация таблицы
            builder.ToTable("Tickets");

            // Конфигурация полей
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200); // Пример ограничения по длине

            builder.Property(x => x.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)"); // Пример для числового поля

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.Property(x => x.FreeSeats)
                   .IsRequired();

            builder.Property(x => x.Location)
                   .IsRequired()
                   .HasMaxLength(200); // Пример ограничения по длине

            // Генерация идентификатора
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
        }
    }
}
