using System;

namespace UnitORM
{
    public static class PredicateFunc
    {
        public static Func<T, bool> True<T>() where T : IEntity
        {
            return a => true;
        }

        public static Func<T, bool> Flase<T>() where T : IEntity
        {
            return a => false;
        }

        public static Func<T, bool> And<T>(this Func<T, bool> left, Func<T, bool> right) where T : IEntity
        {
            return a => left(a) && right(a);
        }

        public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right) where T : IEntity
        {
            return a => left(a) || right(a);
        }
    }
}
