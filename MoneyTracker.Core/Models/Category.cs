using System;
using MoneyTracker.Core.Enums;

namespace MoneyTracker.Core.Models
{
    // модель категории для группировки транзакций, поддерживает древовидную структуру (родительские/дочерние категории)
    public class Category
    {
        public Guid Id { get; set; }                  // уникальный идентификатор

        public string Name { get; set; }             // название категории 

        public string Description { get; set; }       // описание категории

        public TransactionType Type { get; set; }

        public Guid? ParentCategoryId { get; set; }

        // навигационное свойство к родительской категории
        public Category? ParentCategory { get; set; }

        // дата создания категории
        public DateTime CreatedDate { get; set; }

        // системная ли категория (нельзя удалять/редактировать)
        public bool IsSystem { get; set; }

        // порядок отображения в списках
        public int Order { get; set; }

        // активна ли категория (мягкое удаление)
        public bool IsActive { get; set; }

        // есть ли дочерние категории
        public bool HasChildren { get; set; }

        // автоматические свойства (вычисляются на основе типа)

        // иконка категории в зависимости от типа
        public string Icon => Type == TransactionType.Income ? "💰" : "💸";

        // цвет категории в интерфейсе
        public string Color => Type == TransactionType.Income ? "#4CAF50" : "#F44336";

        // конструктор по умолчанию
        public Category()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            IsActive = true;
            Order = 0;
            Name = string.Empty;
            Description = string.Empty;
            HasChildren = false;
        }

        // конструктор с названием и типом
        public Category(string name, TransactionType type) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }

        // полный конструктор
        public Category(string name, TransactionType type, Guid? parentCategoryId, string description, bool isSystem)
            : this(name, type)
        {
            ParentCategoryId = parentCategoryId;
            Description = description ?? string.Empty;
            IsSystem = isSystem;
        }

        // строковое представление для отладки и отображения
        public override string ToString() => $"{Icon} {Name}";
    }
}