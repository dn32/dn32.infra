using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace dn32.infra
{
    [Serializable]
    public class DnContextoDeValidacaoException : Exception
    {
        public bool EhErroDeValidacao => true;

        public List<DnErroDeValidacao> Inconsistencies { get; }

        public bool EhValido => this.Inconsistencies.Count == 0;

        public bool EhInvalido => !this.EhValido;

        public override string Message => string.Join("\n", this.Inconsistencies.Select(x => "* " + (x.MensagemDeGlobalizacao ?? x.Mensagem)).ToArray());

        public void AddInconsistency(DnErroDeValidacao exception) =>
            Inconsistencies.Add(exception);

        public DnContextoDeValidacaoException() : base(string.Empty) =>
            Inconsistencies = new List<DnErroDeValidacao>();

        public void Validate()
        {
            if (EhInvalido)
                throw this;
        }

        protected DnContextoDeValidacaoException(SerializationInfo serializationInfo, StreamingContext streamingContext) =>
            throw new NotImplementedException();
    }
}