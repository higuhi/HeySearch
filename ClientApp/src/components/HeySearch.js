import React from 'react';
import axios from 'axios';
import { trackPromise, usePromiseTracker } from 'react-promise-tracker';
import Loader from 'react-promise-loader';

import HeySearchTable from './HeySearchTable';

class HeySearch extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            searchTerm: "",
            isImageOnly: false,
            isOriginalOnly: false,
            nextToken: "",
            data: [],
            message: ""
        };
        this.search = this.search.bind(this);
    }

    /**
     * Make a search request and load the response data. 
     * @param {bool} useNext true to use next page token 
     */
    async search(useNext=false) {

        this.setState({message: ""}); // clear the message.

        //Construct the URL for the target end node and query 
        const url = this.props.url 
            + "?q=" + this.state.searchTerm 
            + (this.state.isImageOnly ? "&io=1" : "") 
            + (this.state.isOriginalOnly ? "&oo=1" : "")
            + (useNext===true && this.state.nextToken!=="" ? "&nt="+this.state.nextToken : "");

        console.log(`EndPoint URL: ${url}`);

        //Request the search result 
        try {
            const {data} = await trackPromise(axios.get(url));

            // append data if this is next page, otherwise replace the data
            const newItems = (useNext ? [...this.state.data, ...data.items ] : data.items);
            
            this.setState({ data: newItems, nextToken: data.nextPageToken});;
        } catch (error) {            
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

    render() {

        // if there is next token, show "Load More" button for the next page
        let loadMore = "";
        if(this.state.nextToken) {
            loadMore = <button onClick={()=>this.search(true)}>Load More</button>;
        }

        // if there is a message to show, display it 
        let message = "";
        if(this.state.message) {
            message = <div><span>{this.state.message}</span></div>;
        }

        return(
            <div>
                <div>
                    <label htmlFor="io">Search words: </label>
                    <input type="text" placeholder="Enter search words" name="search"
                            value={this.state.searchTerm} 
                            onChange={(e)=>this.setState({searchTerm: e.target.value})} />
                    
                    <br />
                    
                    <input type="checkbox" id="io" name="io" value="1" 
                            defaultChecked={this.state.isImageOnly} 
                            onChange={()=>this.setState({isImageOnly: !this.state.isImageOnly})} />
                    <label htmlFor="io">Search tweets with image only</label>
                    
                    <input type="checkbox" id="oo" name="oo" value="1" 
                            defaultChecked={this.state.isOriginalOnly} 
                            onChange={()=>this.setState({isOriginalOnly: !this.state.isOriginalOnly})} />
                    <label htmlFor="oo">Search original (no retweets)</label>
                    
                    <br />

                    <button disabled={(this.state.searchTerm==="")}
                            onClick={()=>this.search()} >Search</button>
                </div>

                {message}
                
                <Loader promiseTracker={usePromiseTracker} />

                <HeySearchTable data={this.state.data} />
                
                {loadMore}
            </div>
        );
    }
}

export default HeySearch;