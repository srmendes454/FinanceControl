using FinanceControl.Application.Extensions.Utils.Email;
using FinanceControl.Application.Services.Cards.Model.Enum;
using FinanceControl.Application.Services.Transactions.Model;
using FinanceControl.Application.Services.User.Model;
using System;

namespace FinanceControl.Application.Extensions.Utils.SignedBy
{
    public class SignedBy : ISignedBy
    {
        public void SignedByCard(string userName, FamilyMemberModel familyMember, TransactionsModel model, string nameCard, IEmail email, string subjectEmail)
        {
            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = email.TemplateTransactionNotification(userName, model.Name, model.Repetition.ValueInstallment, nameCard, "Cartão de Crédito");
                    var emailSend = email.Send(familyMember.Email, subjectEmail, template);
                }
                catch (Exception) { }
            }
        }

        public void SignedByCardDebit(string userName, FamilyMemberModel familyMember, TransactionsModel model, string nameCard, IEmail email, string subjectEmail)
        {
            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = email.TemplateTransactionNotification(userName, model.Name, model.Repetition.ValueInstallment, nameCard, "Cartão de Débito");
                    var emailSend = email.Send(familyMember.Email, subjectEmail, template);
                }
                catch (Exception) { }
            }
        }

        public void SignedByBankSlip(string userName, FamilyMemberModel familyMember, TransactionsModel model, string name, IEmail email, string subjectEmail)
        {
            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = email.TemplateTransactionNotification(userName, model.Name, model.Repetition.ValueInstallment, name, "Boleto Bancário");
                    var emailSend = email.Send(familyMember.Email, subjectEmail, template);
                }
                catch (Exception) { }
            }
        }

        public void SignedByPix(string userName, FamilyMemberModel familyMember, TransactionsModel model, string name, IEmail email, string subjectEmail)
        {
            if (familyMember.UserId != Guid.Empty)
                model.Assigned = new AssignedModel(familyMember.UserId, familyMember.Name, familyMember.Email);
            else
            {
                model.Assigned = new AssignedModel(familyMember.FamilyId, familyMember.Name, familyMember.Email);
                try
                {
                    var template = email.TemplateTransactionNotification(userName, model.Name, model.Repetition.ValueInstallment, name, "Pix");
                    var emailSend = email.Send(familyMember.Email, subjectEmail, template);
                }
                catch (Exception) { }
            }
        }
    }
}
