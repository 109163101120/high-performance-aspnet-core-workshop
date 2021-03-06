﻿using System;
using System.Buffers;
using GettingThingsDone.Contracts.Dto;
using GettingThingsDone.Contracts.Interface;
using GettingThingsDone.Contracts.Model;
using static GettingThingsDone.Contracts.Interface.ServiceResult;

namespace GettingThingsDone.ApplicationCore.Services
{
    /// <summary>
    /// The legacy exchange action list format is a fixed width format specified as follows:
    /// Title:        100 characters (<see cref="TitleWidth"/>)
    /// Description: 1000 characters (<see cref="DescriptionWidth"/>
    /// In addition, an exchange value can have an arbitrary number of leading and trailing whitespaces.
    /// </summary>
    internal static class LegacyExchangeActionListFormat
    {
        private const int TitleWidth = 100;
        private const int DescriptionWidth = 1000;
        private const int TotalWidth = TitleWidth + DescriptionWidth;

        private const int TitleStart = 0;
 
        public static ServiceResult<ActionListDto> ToActionListDto(string legacyExchangeValue)
        {
            var value = legacyExchangeValue.AsSpan().Trim();
            if (value.Length != TotalWidth)
                return InvalidOperation<ActionListDto>("Invalid legacy exchange value.");

            var title = value.Slice(TitleStart, TitleWidth).Trim();

            return Ok(new ActionListDto
            {
                Name = new string(title)
            });
        }

        public static string ToLegacyFormat(this ActionList actionList)
        {
            var buffer = ArrayPool<char>.Shared.Rent(TotalWidth);

            string result;
            try
            {
                Array.Fill(buffer, ' ');

                actionList.Name?.CopyTo(0, buffer, TitleStart, Math.Min(TitleWidth, actionList.Name.Length));

                result = new string(buffer);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            
            return result;
        }
    }
}