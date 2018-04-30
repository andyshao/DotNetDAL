﻿// -----------------------------------------------------------------------
//  <copyright file="LuceneIndexWriter.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Raven.Server.Exceptions;
using Sparrow.Logging;

namespace Raven.Server.Documents.Indexes.Persistence.Lucene
{
    public class LuceneIndexWriter : IDisposable
    {
        private readonly Logger _logger;

        private IndexWriter _indexWriter;

        private readonly Directory _directory;

        private readonly Analyzer _analyzer;

        private readonly IndexDeletionPolicy _indexDeletionPolicy;

        private readonly IndexWriter.MaxFieldLength _maxFieldLength;

        private readonly IndexWriter.IndexReaderWarmer _indexReaderWarmer;

        public Directory Directory => _indexWriter?.Directory;

        public Analyzer Analyzer => _indexWriter?.Analyzer;

        public LuceneIndexWriter(Directory d, Analyzer a, IndexDeletionPolicy deletionPolicy,
            IndexWriter.MaxFieldLength mfl, IndexWriter.IndexReaderWarmer indexReaderWarmer, DocumentDatabase documentDatabase, IState state)
        {
            _directory = d;
            _analyzer = a;
            _indexDeletionPolicy = deletionPolicy;
            _maxFieldLength = mfl;
            _indexReaderWarmer = indexReaderWarmer;
            _logger = LoggingSource.Instance.GetLogger<LuceneIndexWriter>(documentDatabase.Name);
            RecreateIndexWriter(state);
        }

        public void AddDocument(global::Lucene.Net.Documents.Document doc, Analyzer a, IState state)
        {
            _indexWriter.AddDocument(doc, a, state);
        }

        public void DeleteDocuments(Term term, IState state)
        {
            _indexWriter.DeleteDocuments(term, state);
        }

        public void Commit(IState state)
        {
            try
            {
                _indexWriter.Commit(state);
            }
            catch (SystemException e)
            {
                if (e.Message.StartsWith("this writer hit an OutOfMemoryError"))
                {
                    RecreateIndexWriter(state);
                    throw new OutOfMemoryException("Index writer hit OOM during commit", e);
                }

                throw;
            }

            RecreateIndexWriter(state);
        }

        public long RamSizeInBytes()
        {
            return _indexWriter.RamSizeInBytes();
        }

        public void Optimize(IState state)
        {
            _indexWriter.Optimize(state);
        }

        private void RecreateIndexWriter(IState state)
        {
            try
            {
                DisposeIndexWriter();

                if (_indexWriter == null)
                    CreateIndexWriter(state);
            }
            catch (Exception e)
            {
                throw new IndexWriterCreationException(e);
            }
        }

        private void CreateIndexWriter(IState state)
        {
            _indexWriter = new IndexWriter(_directory, _analyzer, _indexDeletionPolicy, _maxFieldLength, state);
            _indexWriter.UseCompoundFile = false;
            if (_indexReaderWarmer != null)
            {
                _indexWriter.MergedSegmentWarmer = _indexReaderWarmer;
            }
            using (_indexWriter.MergeScheduler)
            {
            }
            _indexWriter.SetMergeScheduler(new SerialMergeScheduler(), state);

            // RavenDB already manages the memory for those, no need for Lucene to do this as well
            _indexWriter.SetMaxBufferedDocs(IndexWriter.DISABLE_AUTO_FLUSH);
            _indexWriter.SetRAMBufferSizeMB(1024);
        }

        private void DisposeIndexWriter(bool waitForMerges = true)
        {
            if (_indexWriter == null)
                return;

            var writer = _indexWriter;
            _indexWriter = null;

            try
            {
                writer.Analyzer.Close();
            }
            catch (Exception e)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Error while closing the index (closing the analyzer failed)", e);
            }

            try
            {
                writer.Dispose(waitForMerges);
            }
            catch (Exception e)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Error when closing the index", e);
            }
        }

        public void Dispose()
        {
            DisposeIndexWriter();
        }

        public void AddIndexesNoOptimize(Directory[] directories, int count, IState state)
        {
            _indexWriter.AddIndexesNoOptimize(state, directories);
        }

        public int NumDocs(IState state)
        {
            return _indexWriter.NumDocs(state);
        }
    }
}
