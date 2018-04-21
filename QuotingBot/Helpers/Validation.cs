using Microsoft.Bot.Builder.FormFlow;
using QuotingBot.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using QuotingBot.Enums;

namespace QuotingBot.Helpers
{
    [Serializable]
    public class Validation
    {
        private static RelayFullCycleMotorService.RelayFullCycleMotorService motorService = new RelayFullCycleMotorService.RelayFullCycleMotorService();
        private Formatter formatter = new Formatter();
        public static Error errorLogging = new Error();
        public Validation() { }

        public ValidateResult ValidateVehicleValue(object value)
        {
            var result = new ValidateResult
            {
                IsValid = false
            };

            if (decimal.TryParse(value.ToString(), out decimal returnValue))
            {
                result.IsValid = true;
                result.Value = Math.Round(returnValue, MidpointRounding.AwayFromZero).ToString();
            }
            else
            {
                result.Feedback = $"The value {value} wasn't valid.  Make sure you enter a number, like €2000.";
            }
            return result;
        }

        public ValidateResult ValidateFirstName(object value)
        {
            var firstName = value.ToString();
            var result = new ValidateResult
            {
                IsValid = false
            };

            if(!string.IsNullOrEmpty(firstName))
            {
                result.IsValid = true;
                result.Value = formatter.CapitilzeFirstLetter(firstName);
            }
            else
            {
                result.Feedback = "You need to provide a first name to continue.";
            }

            return result;
        }

        public ValidateResult ValidateTown(object value)
        {
            var town = value.ToString();

            var result = new ValidateResult
            {
                IsValid = false
            };

            if (motorService.GetAreaCodeList().Contains(town))
            {
                result.IsValid = true;
                result.Value = value.ToString();
            }
            else
            {
                result.Feedback = $"Oh dear...I don't recognise that town.  Can you check the spelling of '{town}' or try an area close by? Thanks {Emoji.ThumbsUp}";
            }

            return result;
        }

        public ValidateResult ValidateCounty(object value)
        {
            var county = value.ToString();

            var result = new ValidateResult
            {
                IsValid = false
            };

            if (motorService.GetCountyCodeList().Contains(county))
            {
                result.IsValid = true;
                result.Value = value.ToString();
            }
            else
            {
                result.Feedback = $"Oh dear...I don't recognise that county.  Can you check the spelling of '{county}' or try an area close by? Thanks {Emoji.ThumbsUp}";
            }

            return result;
        }

        public ValidateResult ValidateYearBuilt(object value)
        {
            var result = new ValidateResult
            {
                IsValid = false
            };

            if (IsYearBuiltValid(value.ToString()) && int.TryParse(value.ToString(), out int returnValue))
            {
                result.IsValid = true;
                result.Value = returnValue.ToString();
            }
            else
            {
                result.Feedback = $"The value {value} wasn't valid.  Make sure you enter a year in 'YYYY' format, like 2018.";
            }
            return result;
        }

        private bool IsYearBuiltValid(string yearBuilt)
        {
            string validYearPattern = @"^[0-9]{4}$";
            Regex validYear = new Regex(validYearPattern);

            return validYear.IsMatch(yearBuilt);
        }

        public ValidateResult ValidateLastName(object value)
        {
            var lastName = value.ToString();
            var result = new ValidateResult
            {
                IsValid = false
            };

            if (!string.IsNullOrEmpty(lastName))
            {
                result.IsValid = true;
                result.Value = formatter.CapitilzeFirstLetter(lastName);
            }
            else
            {
                result.Feedback = "You need to provide a last name to continue.";
            }

            return result;
        }

        public ValidateResult ValidateAreaVehicleIsKept(object value)
        {
            var area = value.ToString();

            var result = new ValidateResult
            {
                IsValid = false
            };

            if (motorService.GetAreaCodeList().Contains(area) || motorService.GetCountyCodeList().Contains(area))
            {
                result.IsValid = true;
                result.Value = value.ToString();
            }
            else
            {
                result.Feedback = $"Oh dear...I don't recognise that area.  Can you check the spelling of '{area}' or try an area close by? Thanks {Emoji.ThumbsUp}";
            }

            return result;
        }

        public ValidateResult ValidateDateOfBirth(object value)
        {
            var result = new ValidateResult
            {
                IsValid = false
            };

            try
            {
                CultureInfo culture = new CultureInfo("en-GB");
                var date = Convert.ToDateTime(value, culture);

                if(IsProposerOfLegalDrivingAge(date))
                {
                    result.IsValid = true;
                    result.Value = value.ToString();
                }
                else
                {
                    result.Feedback = "Sorry, but we can't quote for anyone under the age of 17";
                    return result;
                }
            }
            catch (Exception ex)
            {
                errorLogging.Log(DateTime.Now.ToShortDateString(), ex.InnerException.ToString());
                throw;
            }

            return result;
        }

        public ValidateResult ValidateEmailAddress(object value)
        {
            var result = new ValidateResult
            {
                IsValid = false
            };

            if(IsEmailAddressValid(value.ToString()))
            {
                result.IsValid = true;
                result.Value = value.ToString();
            }
            else
            {
                result.Feedback = $"Please enter a valid email address {Emoji.Email}";
            }

            return result;
        }

        private bool IsProposerOfLegalDrivingAge(DateTime dateOfBirth) => dateOfBirth <= DateTime.Now.AddYears(-17);

        private bool IsEmailAddressValid(string emailAddress)
        {
            string validEmailPattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b";
            Regex validEmailAddress = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

            return validEmailAddress.IsMatch(emailAddress);
        }

        public ValidateResult ValidateNumberOfBedrooms(object value)
        {
            var result = new ValidateResult
            {
                IsValid = false
            };

            if (int.TryParse(value.ToString(), out int returnValue))
            {
                if (returnValue >= 0 && returnValue <= 9)
                {
                    result.IsValid = true;
                    result.Value = returnValue.ToString();
                }
                else
                {
                    result.Feedback = $"Sorry, but we can't quote for {value} bedrooms.";
                }
            }
            else
            {
                result.Feedback = $"Sorry, {value} wasn't valid number of bedrooms.";
            }
            return result;
        }
    }
}