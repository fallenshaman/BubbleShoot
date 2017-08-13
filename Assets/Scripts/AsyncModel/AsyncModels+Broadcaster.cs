using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AsyncModel
{
    public class PromiseBroadcaster< T >
    {
        public class MIC
        {
            public PromiseBroadcaster< T > Broadcaster { get; private set; }

            public MIC() { Broadcaster = new PromiseBroadcaster< T >(); }

            public Promise Say( T speech )
            {
                var jobs = Broadcaster.listeners.Select( e => Promise.CreateJob( e, speech ) ).ToArray();
                return Promise.Serialize( jobs );
            }
        }
	
		private List< Func< T, Promise > > listeners;
        		
		private PromiseBroadcaster()
		{
            listeners = new List< Func< T, Promise > >();
        }
		
		public Action Subscribe( Func< T, Promise > listener )
		{			
			listeners.Add( listener );
			return ()=>{ listeners.Remove( listener ); };
		}
		
		public Action SubscribeOnce( Func< T, Promise > listener )
		{
			Action canceler = null;
			Func< T, Promise > wrappedListener = ( word ) =>
			{
				canceler();				
				return listener( word );
			};
			canceler = Subscribe( wrappedListener );
			return canceler;
		}
    }

    public class PromiseBroadcaster
    {
        public class MIC
        {
            public PromiseBroadcaster Broadcaster { get; private set; }

            public MIC()
            {
                Broadcaster = new PromiseBroadcaster();
            }

            public Promise Say()
            {
                var jobs = Broadcaster.handlers.Select( e => Promise.CreateJob( e ) ).ToArray();
                return Promise.Serialize( jobs );
            }
        }
	
		private List< Func< Promise > > handlers;
        		
		private PromiseBroadcaster()
		{
            handlers = new List< Func< Promise > >();
        }		
		
		public Action Subscribe( Func< Promise > handler )
		{			
			handlers.Add( handler );
			return ()=>{ handlers.Remove( handler ); };
		}
		
		public Action SubscribeOnce( Func< Promise > handler )
		{
			Action canceler = null;
			Func< Promise > wrappedHandler = () =>
			{
				canceler();				
				return handler();
			};
			canceler = Subscribe( wrappedHandler );
			return canceler;
		}
    }
	
	public class Broadcaster< T >
	{
		public class MIC
		{
			public readonly Broadcaster< T > Broadcaster;
			
			public MIC()
			{
				Broadcaster = new Broadcaster< T >();
			}
			
			public void Say( T word )
			{
				foreach( var handler in Broadcaster.handlers.ToArray() )
				{
					handler( word );
				}
			}

            public IEnumerable< Action< T > > Enumerable
            {
                get
                {
                    foreach( var handler in Broadcaster.handlers )
                        yield return handler;
                }
            }
		}
	
		private List< Action< T > > handlers;
		
		private Broadcaster()
		{
			handlers = new List< Action< T > >();
		}				
		
		public Action Subscribe( Action< T > handler )
		{			
			handlers.Add( handler );
			return ()=>{ handlers.Remove( handler ); };
		}
		
		public Action SubscribeOnce( Action< T > handler )
		{
			Action canceler = null;
			Action< T > wrappedHandler = ( word )=>
			{
				canceler();				
				handler( word );
			};
			canceler = Subscribe( wrappedHandler );
			return canceler;
		}
	}
	
	// typeless
	public class Broadcaster
	{
		public class MIC
		{
			public readonly Broadcaster Broadcaster;
			
			private Broadcaster< object >.MIC impl;
			
			public MIC()
			{				
				impl = new Broadcaster< object >.MIC();
				Broadcaster = new Broadcaster( impl.Broadcaster );
			}
			
			public void Say()
			{
				impl.Say( null );
			}
		}
		
		private Broadcaster< object > impl;
		
		private Broadcaster( Broadcaster< object > _impl )
		{
			impl = _impl;
		}				
		
		public Action Subscribe( Action handler )
		{
			return impl.Subscribe( ( word )=>{ handler(); } );
		}
		
		public Action SubscribeOnce( Action handler )
		{
			return impl.SubscribeOnce( ( word )=>{ handler(); } );
		}
	}
}