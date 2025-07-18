﻿using System;
using System.Collections.Generic;
using System.Xml;
using ZLinq;

namespace PJH.Utility.Extensions
{
    public static class ListExtensions
    {
        static Random rng;

        /// <summary>
        /// 컬렉션이 null이거나 요소가 하나도 없는지 확인합니다.
        /// 컬렉션 전체를 열거하여 개수를 세지 않고 판단합니다.
        /// </summary>
        /// <param name="list">검사할 리스트입니다.</param>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count <= 0;
        }

        /// <summary>
        /// 원본 리스트의 복사본인 새 리스트를 생성합니다.
        /// </summary>
        /// <param name="list">복사할 원본 리스트입니다.</param>
        /// <returns>원본 리스트를 복사한 새 리스트를 반환합니다.</returns>
        public static List<T> Clone<T>(this IList<T> list)
        {
            return list.AsValueEnumerable().ToList();
        }

        /// <summary>
        /// 리스트 내에서 지정한 두 인덱스의 요소를 서로 교환합니다.
        /// </summary>
        /// <param name="list">대상 리스트입니다.</param>
        /// <param name="indexA">첫 번째 요소의 인덱스입니다.</param>
        /// <param name="indexB">두 번째 요소의 인덱스입니다.</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        /// <summary>
        /// Fisher-Yates 알고리즘의 Durstenfeld 구현을 사용하여 리스트 요소를 섞습니다.
        /// 이 메서드는 입력 리스트를 직접 수정하며, 모든 순열이 동일한 확률로 발생합니다.
        /// 메서드 체이닝을 위해 섞인 리스트를 반환합니다.
        /// 참고: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </summary>
        /// <param name="list">섞을 리스트입니다.</param>
        /// <typeparam name="T">리스트 요소 타입입니다.</typeparam>
        /// <returns>섞인 리스트를 반환합니다.</returns>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (rng == null) rng = new Random();
            int count = list.Count;
            while (count > 1)
            {
                --count;
                int index = rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }

            return list;
        }
    }
}