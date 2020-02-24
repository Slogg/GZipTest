using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.Domain
{
    internal sealed class DecompressorStrategy : IGZipStrategy
    {
        /// <summary>
        /// <see cref="IGZipStrategy.Handle"/>
        /// </summary>
        public void Handle()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Read"/>
        /// </summary>
        public void Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IGZipStrategy.Write"/>
        /// </summary>
        public void Write()
        {
            throw new NotImplementedException();
        }
    }
}
