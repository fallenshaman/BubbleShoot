using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AsyncModel
{
	public class Promise
	{
		public delegate void Job( Handle handle );

		public enum eState
		{
			Created,
			Succeeded,
			Failed,
		}

		public class Handle
		{
			public eState State;
			public readonly Promise Promise;

			public Handle( Promise promise )
			{
				Promise = promise;
				State = eState.Created;
			}	

			// duplicated terminate allowed, but do nothing.
			public void Terminate( bool succeeded )
			{
				if( State == eState.Created ) 
				{
					State = succeeded ? eState.Succeeded : eState.Failed;
					Promise.onHandleTerminated( this );
				}
			}

			public void DoSuccess(){ Terminate( true ); }
			public void DoFail(){ Terminate( false ); }
		}

		public eState State{ get; private set; }
		
		private Handle[] handles;

		private Queue< Action > onThens;
		private Queue< Action > onElses;
		private Queue< Action > onFinals;

		#region PRESETS
		private static Promise succeeded = Parallelize( ( token )=>{ token.Terminate( true ); } );
		private static Promise failed = Parallelize( ( token )=>{ token.Terminate( false ); } );
		public static Promise Succeeded{ get{ return succeeded; } }
		public static Promise Failed{ get{ return failed; } }
		#endregion

		public static Job CreateJob( Func< Promise > task )
		{
			return ( handle )=>
			{
				var promise = task();
				promise.Then( handle.DoSuccess );
				promise.Else( handle.DoFail );
			};
		}

        public static Job CreateJob< T >( Func< T, Promise > task, T parameter )
        {
            return ( handle ) =>
            {
                var promise = task( parameter );
                promise.Then( handle.DoSuccess );
                promise.Else( handle.DoFail );
            };
        }
		
		#region STATIC CREATORS	

		public static Handle Start()
		{
			Handle result = null;
			
			Promise.Parallelize( handle =>
			{
				result = handle;
			} );

			return result;
		}

		public static Promise Parallelize( params Job[] jobs )
		{
			Promise promise = new Promise();
			promise.start( jobs );
			return promise;
		}

		public static Promise Serialize( params Job[] jobs )
		{
			var resultHandle = Promise.Start();

			var jobQueue = new Queue< Job >( jobs );

			System.Action continueJob = null;

			continueJob = ()=>
			{
				if( jobQueue.Count > 0 )
				{
					var job = jobQueue.Dequeue();

					var promise = new Promise();
					promise.start( new Job[]{ job } );
					promise.Then( continueJob );
					promise.Else( resultHandle.DoFail );
				}
				else
				{
					resultHandle.DoSuccess();
				}
			};

			continueJob();

			return resultHandle.Promise;
		}

		public static Promise Wait( params Promise[] promises )
		{
			Promise promise = new Promise();
			promise.wait( promises );
			return promise;
		}
		
		#endregion

		#region INSTANT CALLBACK REGISTRAS

		// return self
		public Promise Then( Action onThen )
		{			
			if( State == eState.Created ) 
			{
				onThens.Enqueue( onThen );
			} 
			else if( State == eState.Succeeded )
			{
				onThen();
			}
			return this;
		}

		// return new promise
		public Promise ThenStart( params Job[] jobs )
		{
			Promise promise = new Promise();
			Then( ()=>{ promise.start( jobs ); } );
			return promise;
		}

		// return new promise
		public Promise ThenStart( params Func< Promise >[] tasks )
		{
			return ThenStart( tasks.Select( task => CreateJob( task ) ).ToArray() );
		}

		// return self
		public Promise Else( Action onElse )
		{
			if( State == eState.Created ) 
			{
				onElses.Enqueue( onElse );
			} 
			else if( State == eState.Failed )
			{
				onElse();
			}
			return this;
		}
		
		// return new promise
		public Promise ElseStart( params Job[] jobs )
		{
			Promise promise = new Promise();
			Else( ()=>{ promise.start( jobs ); } );
			return promise;
		}

		// return new promise
		public Promise ElseStart( params Func< Promise >[] tasks )
		{
			return ElseStart( tasks.Select( task => CreateJob( task ) ).ToArray() );
		}
		
		// return self
		public Promise Final( Action onFinal )
		{
			if( State == eState.Created ) 
			{
				onFinals.Enqueue( onFinal );
			} 
			else 
			{
				onFinal();
			}
			return this;
		}
		
		// return new promise
		public Promise FinalStart( params Job[] jobs )
		{
			Promise promise = new Promise();
			Final( ()=>{ promise.start( jobs ); } );
			return promise;
		}

		// return new promise
		public Promise FinalStart( params Func< Promise >[] tasks )
		{
			return FinalStart( tasks.Select( task => CreateJob( task ) ).ToArray() );
		}
		#endregion
		
		private void createHandles( int count )
		{
			handles = new Handle[ count ];
			
			for( int i = 0; i < count; ++i )
			{
				handles[ i ] =new Handle( this );
			}
		}

		private void start( IEnumerable< Job > jobs )
		{
			Job[] jobsArray = jobs.ToArray();

			if( jobsArray.Length == 0 )
			{
				jobsArray = new Job[]{ ( handle )=>{ handle.Terminate( true ); } };
			}
			
			// must construct handles first.
			createHandles( jobsArray.Length );
			
			for( int i = 0; i < jobsArray.Length; ++i )
			{
				jobsArray[ i ]( handles[ i ] );
			}
		}
		
		private void wait( IEnumerable< Promise > promises )
		{
			Promise[] promisesArray = promises.ToArray();

			if( promisesArray.Length == 0 )
			{
				promisesArray = new Promise[]{ Succeeded };
			}
			
			// must construct handles first.
			createHandles( promisesArray.Length );
			
			Handle[] handlesArray = handles.ToArray();
			for( int i = 0; i < promisesArray.Length; ++i )
			{
				Handle handle = handlesArray[ i ];
				Promise promise = promisesArray[ i ];
				promise.Then( ()=>{ handle.Terminate( true ); } );
				promise.Else( ()=>{ handle.Terminate( false ); } );
			}
		}

		private Promise()
		{
			State = eState.Created;			
			onThens = new Queue< Action >();
			onElses = new Queue< Action >();
			onFinals = new Queue< Action >();
		}

		private void onHandleTerminated( Handle handle )
		{
			if( State == eState.Created ) 
			{			
				if( handle.State == eState.Succeeded ) 
				{
					if( handles.All( elem => elem.State == eState.Succeeded ) )
					{
						State = eState.Succeeded;
						flushJobs( onThens );
						flushJobs( onFinals );
					}
				} 
				else 
				{
					// assume that handle.State == eState.Failed
					State = eState.Failed;
					flushJobs( onElses );
					flushJobs( onFinals );
				}
			}
		}

		private static void flushJobs( Queue< Action > jobs )
		{
			while( jobs.Count > 0 )
			{
				jobs.Dequeue()();
			}										
		}

        internal object Then()
        {
            throw new NotImplementedException();
        }
    }
	
	public class Delivery< T >
	{
		public class Handle
		{
			public readonly Delivery< T > Delivery;
			
			public Handle( Delivery< T > delivery )
			{
				Delivery = delivery;
			}

			public void Terminate( bool succeeded, T item )
			{
				if( Delivery.promiseHandle.State == Promise.eState.Created )
				{
					Delivery.Item = item;
					Delivery.promiseHandle.Terminate( succeeded );
				}
			}
		}
		
		public Promise Promise{ get{ return promiseHandle.Promise; } }

		public T Item{ get; private set; }

		private Promise.Handle promiseHandle;
		
		public static Handle Start()
		{	
			var delievery = new Delivery< T >();
			return new Handle( delievery );
		}
		
		private Delivery()
		{			
			promiseHandle = Promise.Start();
		}
		
		// return self
		public Delivery< T > Then( Action< T > onDeliver )
		{
			Promise.Then( ()=>{ onDeliver( Item ); } );
			return this;
		}

        public Delivery< T > Else( Action< T > onDeliver )
        {
            Promise.Else( () => { onDeliver( Item ); } );
            return this;
        }
	}

	public class JobDispatcher
	{
		public class Job
		{
			public readonly float TargetTime;
			public readonly System.Action Action;
			public readonly int Index;
		
			private JobDispatcher holder;
		
			public Job( JobDispatcher _holder, float targetTime, System.Action action )
			{
				holder = _holder;
				TargetTime = targetTime;
				Action = action;
				Index = holder.counter++;
			}
		
			public void Cancel()
			{
				holder.jobs.Remove( this );
			}
		}
	
		private List< Job > jobs;
		private int counter;
	
		public JobDispatcher()
		{
			counter = 0;
			jobs = new List< Job >();
		}
	
		public Job AddJob( float targetTime, System.Action action )
		{
			Job job = new Job( this, targetTime, action );
		
			// insertion sort( stable )
			bool added = false;
			for( int i = 0; i < jobs.Count; ++i )
			{
				if( job.TargetTime < jobs[ i ].TargetTime )
				{
					jobs.Insert( i, job );
					added = true;
					break;
				}
			}
			if( !added )
			{
				jobs.Add( job );
			}
		
			return job;
		}
	
		public void DispatchJobs( float time )
		{
			// flush jobs
			while( jobs.Count > 0 )
			{
				Job job = jobs[ 0 ];
				if( job.TargetTime < time )
				{
					jobs.RemoveAt( 0 );
					job.Action();
				}
				else
				{
					break;
				}
			}
		}
	}

    public class ZeroOneCounter
    {
        public delegate Promise ZeroOneEventHandler();

        public event ZeroOneEventHandler Changed01;
        public event ZeroOneEventHandler Changed10;

        public Broadcaster< int > Changed { get { return changedMIC.Broadcaster; } }
        private Broadcaster< int >.MIC changedMIC = new Broadcaster< int >.MIC();

        public int Count { get; private set; }

        public Promise Plus()
        {
            var plus = Promise.Start();

            Count += 1;
            changedMIC.Say( Count );

            if( Count == 1 && null != Changed01 )
                Changed01().Then( plus.DoSuccess );
            else
                plus.DoSuccess();

            return plus.Promise;
        }

        public Promise Minus( bool makeZero = false )
        {
            var minus = Promise.Start();
            
            if( makeZero == true )
                Count = 0;
            else
                Count -= 1;
            changedMIC.Say( Count );

            if( Count == 0 && null != Changed10 )
                Changed10().Then( minus.DoSuccess );
            else
                minus.DoSuccess();


            return minus.Promise;
        }
    }

	public class Token
	{
		public class Inspector : UnityEngine.MonoBehaviour
		{
			public Token Token;
			public string[] StackTraces;
			
			IEnumerator Start()
			{
				while( true )
				{
					if( Token != null )
					{
						StackTraces = Token.records.Select( token => token.StackTrace ).ToArray();
						yield return new UnityEngine.WaitForSeconds( 5.0f );
					}
				}
			}
		}
		
		public class AcquireRecord
		{
			public string StackTrace;
		}
		
		public int Count{ get{ return records.Count; } }

		public AsyncModel.Broadcaster Count01{ get{ return count01MIC.Broadcaster; } }
		public AsyncModel.Broadcaster Count10{ get{ return count10MIC.Broadcaster; } }
		
		private AsyncModel.Broadcaster.MIC count01MIC = new AsyncModel.Broadcaster.MIC();		
		private AsyncModel.Broadcaster.MIC count10MIC = new AsyncModel.Broadcaster.MIC();		

		private List< AcquireRecord > records = new List< AcquireRecord >();
		
		public Token()
		{	
		}

        public string StackTrace(int count)
        {
            string[] stackTraces = UnityEngine.StackTraceUtility.ExtractStackTrace().Split('\n');
            string result = "";
            for (int i = 0; i < count; ++i)
            {
                // 0 means current frame, 1 means requester frame, 2 means requested frame
                int stackDepth = 2 + i;
                if (stackDepth < stackTraces.Length)
                {
                    result += "\n" + stackTraces[stackDepth];
                }
            }
            result += "\n<TRACE END>";
            return result;
        }

        public System.Action Acquire()
		{
			var record = new AcquireRecord()
			{
				StackTrace = StackTrace( 10 ),
			};

			records.Add( record );

			if( Count == 1 )
			{
				count01MIC.Say();
			}

			return ()=>
			{
				records.Remove( record );

				if( Count == 0 )
				{
					count10MIC.Say();
				}
			};
		}

		public void Clear()
		{
			records.Clear();
			count10MIC.Say();
		}
	}
    
    public class NotifyProperty< T >
    {
        public struct ChangedValue< Type >
        {
            public Type Previous;
            public Type Current;
        }

        public Broadcaster< ChangedValue< T > > OnChangedValue { get { return changedValueMIC.Broadcaster; } }
        private Broadcaster< ChangedValue< T > >.MIC changedValueMIC = new Broadcaster< ChangedValue< T > >.MIC();
        private T @value;

        public T Value
        {
            get { return @value; }
            set
            {
                if( !this.@value.Equals( value ) )
                {
                    changedValueMIC.Say( new ChangedValue< T >() { Previous = this.@value, Current = value } );
                }

                this.@value = value;
            }
        }

        static public NotifyProperty< T > New( T value )
        {
            var instance = new NotifyProperty< T >();
            instance.@value = value;
            return instance;
        }

        private NotifyProperty() {}
    }
}
