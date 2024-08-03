using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class RandomUtils
{
    public static T RandomSelect<T>(IList<T> elements)
    {
        return RandomSelect(elements, new System.Random());
    }

    public static T RandomSelect<T>(IList<T> elements, System.Random rng)
    {
        int randomIndex = rng.Next(0, elements.Count);
        return elements[randomIndex];
    }

    public static T WeightedRandomSelect<T>(IList<WeightedElement<T>> weightedElements)
    {
        return WeightedRandomSelect(weightedElements, new System.Random());
    }

    public static T WeightedRandomSelect<T>(IList<WeightedElement<T>> weightedElements, System.Random rng)
    {
        var totalWeight = weightedElements.Sum(x => x.weight);
        double randomValue = rng.NextDouble() * totalWeight;
        foreach (var weightedElement in weightedElements)
        {
            randomValue -= weightedElement.weight;
            if (randomValue <= 0)
            {
                var element = weightedElement.element;
                return element;
            }
        }
        // Should not reach this point if weights added up to 1
        return weightedElements[0].element;
    }
}

[Serializable]
public struct WeightedElement<T>
{
    public T element;
    public float weight;

    public WeightedElement(T element, float weight)
    {
        this.element = element;
        this.weight = weight;
    }
}