import React, { useEffect, useState }from 'react';
import axios from 'axios';
import { trackPromise, usePromiseTracker } from 'react-promise-tracker';
import Loader from 'react-promise-loader';
import styled from 'styled-components';
import MediaQuery from 'react-responsive';

import HeySearchTable from './HeySearchTable';
import HeySearchCard from './HeySearchCard';

const Styles = styled.div`
    padding: 1rem;
    .input_label_option {
        margin-left: 1rem;
    }
    .input_search{
        margin-bottom: 0.5rem;
        font-size: 16px;
    }
    .button_more {
        border: none;
        background: none;
        padding: 0;
        text-decoration: underline;
        cursor: pointer;
        color: blue;
    }
    .center {
        text-align:center; 
    }
    .message {
        color: red;
    }
`;

/**
 * React component to handle "Load More". 
 * This component will use the promise tracker and window scroll to change 
 * the displayed text and to trigger search loading additional search results. 
 * @param {Object} props - react props
 * @param {string} props.nextToken - the next page token to be used
 * @param {Object} props.data - the current search result data
 * @param {function} props.loadFunc - the function that triggers search 
 */
const LoadMore = (props) => {
    const { nextToken, loadFunc, data } = props;
    const { promiseInProgress } = usePromiseTracker(); //this will be true if search is in progress
    const [ windowBottom, setWindowBottom ] = useState(Number.MAX_SAFE_INTEGER); //browser's scroll position 
    const [ clientHeight, setClientHeight ] = useState(0); //client window size

    // A scroll handler to keep track of scroll position and cient window size
    const handleScroll = (event) => {
        setWindowBottom(document.documentElement.getBoundingClientRect().bottom);
        setClientHeight(document.documentElement.clientHeight);
    };

    // set a scroll listener to track the scrolling
    useEffect(() => {
        window.addEventListener('scroll', handleScroll, true);
        return (()=>{window.removeEventListener('scroll', handleScroll)});
    }, []);

    // trigger load more when reaching the end 
    useEffect(() => {
        // trigger the load func only if loading is not in progress, there is a next token 
        // and the scroll position is at the end of browser
        if(!promiseInProgress && nextToken && (windowBottom<clientHeight+50)) {
            setWindowBottom(Number.MAX_SAFE_INTEGER); // pretend the scroll is at the top to prevent triggering multiple times.
            loadFunc();
        }
    }, [windowBottom, promiseInProgress, clientHeight, nextToken, loadFunc])

    if(promiseInProgress) {
        return <div className="center"><span>loading...</span></div>
    } else if(nextToken) {
        return <div className="center"><button className="button_more" onClick={()=>loadFunc()}>Load More</button></div>;
    } else if(data.length!==0) {
        return <div className="center"><span>No more results</span></div>;
    } else {
        return "";
    }
}

/**
 * React component for HeySearch
 */
class HeySearch extends React.Component {

    /**
     * @constructor
     * @param {Object} props - react props
     */
    constructor(props) {
        super(props);
        this.state = {
            searchTerm: "",
            isImageOnly: false,
            includeRetweet: false,
            nextToken: "",
            data: [],
            message: ""
        };
        this.search = this.search.bind(this);
    }

    /**
     * Makes a search request and load the response data. 
     * @param {bool} useNext true to use next page token 
     */
    async search(useNext=false) {

        // clear old message.
        this.setState({message: ""}); 

        //Construct the URL to request the search
        const url = this.props.url 
            + "?q=" + encodeURIComponent(this.state.searchTerm.trim()) 
            + (this.state.isImageOnly ? "&io=1" : "") 
            + (this.state.includeRetweet ? "" : "&oo=1")
            + (useNext===true && this.state.nextToken!=="" ? "&nt="+this.state.nextToken : "");

        console.log(`EndPoint URL: ${url}`);

        // Request the search result 
        try {
            const {data} = await trackPromise(axios.get(url));

            // append data if this is next page, otherwise replace the data
            let newItems; 
            if(useNext) {
                // the "load more" on scroll may trigger multiple time in some cases, 
                // so check the last IDs of the data to avoid duplicates. 
                if(this.state.data[this.state.data.length-1].id === data.items[data.items.length-1].id) {
                    console.log("Ignoring duplicated responses.");
                    newItems = [...this.state.data]; 
                } else {
                    newItems = [...this.state.data, ...data.items];
                }
            } else {
                newItems = data.items;
            }
            
            this.setState({ data: newItems, nextToken: data.nextPageToken});;
        } catch (error) {     
            // Set an apporopriate message depending on the response status
            let message = "";
            if (error.response) {
                if(error.response.status===404) {
                    message = (useNext ? "There are no more results" : "No results found");
                } else {
                    message = `Unexpected server response ${error.response.status}`;
                }
            } else {
                message = `Something went wrong: ${error.message}`;
            }

            this.setState({ data: [], message: message, nextToken: ""});
        }
    }

    /**
     * Renders HeySearch component which consists: 
     * 1) Input form for search keywords and options
     * 2) The search result 
     * 3) LoadMore component 
     */
    render() {

        // if there is a message to show, display it 
        let message = "";
        if(this.state.message) {
            message = <div><span className="message">{this.state.message}</span></div>;
        }

        return(
            <div>
                <Loader promiseTracker={usePromiseTracker} />
                
                <Styles>
                    <div>
                        <form onSubmit={(event)=>{event.preventDefault(); this.search();}} > 
                            <label className="input_label_search" htmlFor="io">Search words:&nbsp;</label>
                            <input className="input_search" type="text" placeholder="Enter search words" name="search"
                                    value={this.state.searchTerm} 
                                    onChange={(e)=>this.setState({searchTerm: e.target.value, nextToken: ""})} />
                            
                            <br />
                            
                            <input className="input_label_option" type="checkbox" id="io" name="io" value="1" 
                                    defaultChecked={this.state.isImageOnly} 
                                    onChange={()=>this.setState({isImageOnly: !this.state.isImageOnly})} />
                            <label htmlFor="io">&nbsp;Search tweets with image only</label>
                            
                            <br />

                            <input className="input_label_option" type="checkbox" id="oo" name="oo" value="1" 
                                    defaultChecked={this.state.includeRetweet} 
                                    onChange={()=>this.setState({includeRetweet: !this.state.includeRetweet})} />
                            <label htmlFor="oo">&nbsp;Include retweets</label>
                            
                            <br />

                            <input type="submit" value="Search" disabled={(this.state.searchTerm.trim()==="")} />
            
                            {message}
                        </form>
                    </div>
                </Styles>

                <MediaQuery query="(max-width: 767px)">
                    <HeySearchCard data={this.state.data} />
                </MediaQuery>
                <MediaQuery query="(min-width: 767px)">
                    <HeySearchTable data={this.state.data} />
                </MediaQuery>


                <Styles>                
                    <LoadMore nextToken={this.state.nextToken} data={this.state.data} loadFunc={() => {this.search(true)}} />
                </Styles>
            </div>
        );
    }
}

export default HeySearch;