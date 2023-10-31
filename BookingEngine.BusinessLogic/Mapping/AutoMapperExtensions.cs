//using AutoMapper;

//namespace BookingEngine.BusinessLogic.Mapping;

//public static class AutoMapperExtensions
//{
//    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
//        this IMappingExpression<TSource, TDestination> expression)
//    {
//        Type sourceType = typeof(TSource);
//        Type destinationType = typeof(TDestination);

//        var existingMaps = Mapper.Instance.ConfigurationProvider
//            .FindTypeMapFor<TSource, TDestination>();

//        if (existingMaps != null)
//        {
//            foreach (var property in destinationType.GetProperties())
//            {
//                if (sourceType.GetProperty(property.Name) == null &&
//                    !existingMaps.PropertyMaps.Any(pm => pm.DestinationProperty.Name == property.Name))
//                {
//                    IgnoreDestinationProperty(expression, property);
//                }
//            }
//        }

//        return expression;
//    }

//    private static void IgnoreDestinationProperty<TSource, TDestination>(
//        IMappingExpression<TSource, TDestination> expression, System.Reflection.PropertyInfo property)
//    {
//        Expression<Func<TDestination, object>> destinationMember = dest => property.GetValue(dest);
//        expression.ForMember(destinationMember, opt => opt.Ignore());
//    }
//}