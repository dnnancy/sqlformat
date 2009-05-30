﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class FormattingEngine
    {
        private Dictionary<Type, Type> _formatters;
        private IStatement _statement;

        /// <summary>
        /// Initializes a new instance of the FormattingEngine class.
        /// </summary>
        public FormattingEngine()
        {
            TabSize = 4;
            UseTabChar = false;

            _formatters = new Dictionary<Type, Type>
            {
                { typeof( SelectStatement ), typeof( SelectStatementFormatter ) },
//                { typeof( CreateTableStatement ), typeof( CreateTableStatementFormatter ) }
//                { typeof( UpdateStatement ), typeof( UpdateStatementFormatter ) }
            };
        }

        public string Execute( string sql )
        {
            string indent = UseTabChar ? "\t" : new string( ' ', TabSize );

            var outSql = new StringBuilder();
            _statement = ParserFactory.Execute( sql );

            // this is a quick and dirty service locator that maps statements to formatters
            var formatterType = _formatters[ _statement.GetType() ];
            
            var formatter = Activator.CreateInstance( formatterType, indent, 0, outSql, _statement ) as IStatementFormatter;

            if ( formatter == null )
                throw new Exception( "Formatter not implemented" );

            formatter.Execute();
            return outSql.ToString();
        }

        public int TabSize { get; set; }
        public bool UseTabChar { get; set; }
    }
}