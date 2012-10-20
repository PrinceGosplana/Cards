using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StoreCard.Models
{
    [MetadataType(typeof(CardMetadata))]
    public partial class Card
    {

    }
    public class CardMetadata
    {
        [Required(ErrorMessage = "Amount is required")]
        [DisplayName("Amount in the card")]
        public decimal Amount { get; set; }
    }
}